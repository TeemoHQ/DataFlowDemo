using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Test.Entity;
using Test.Respository;

namespace Test
{
    public class BatchBlockPipeline<T>
    {
        /// <summary>
        /// 批处理块
        /// </summary>
        private BatchBlock<T> _batchBlock;
        /// <summary>
        /// 批处理执行块
        /// </summary>
        private ActionBlock<T[]> _actionBlock;
        /// <summary>
        /// 是否为定时触发
        /// </summary>
        private bool _timeTrigger;
        /// <summary>
        /// 定时触发时候用到的连接块
        /// </summary>
        private TransformBlock<T, T> _transformBlock;
        /// <summary>
        /// 定时触发器
        /// </summary>
        private readonly Timer _timer;

        /// <summary>
        /// 基本构造函数
        /// </summary>
        /// <param name="batchSize">每次处理的数据量</param>
        /// <param name="action">执行委托方法</param>
        /// <param name="boundedCapacity">最大处理的数据量 默认 int.MaxValue 2147483647</param>
        /// <param name="maxDegreeOfParallelism">最大并行量 默认1</param>
        /// <param name="timeTrigger">定时触发批处理 默认不处理， 设置大于0则处理，秒级别</param>
        public BatchBlockPipeline(int batchSize, Action<T[]> action, int boundedCapacity = int.MaxValue, int maxDegreeOfParallelism = 1, int timeTrigger = 0)
        {
            _batchBlock = new BatchBlock<T>(batchSize, new GroupingDataflowBlockOptions() { BoundedCapacity = boundedCapacity });
            _actionBlock = new ActionBlock<T[]>(data => action(data), new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxDegreeOfParallelism });
            _batchBlock.LinkTo(_actionBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            _batchBlock.Completion.ContinueWith(delegate { _actionBlock.Complete(); });
            if (timeTrigger > 0)
            {
                _timeTrigger = true;
                _timer = new Timer(state => 
                { 
                    _batchBlock.TriggerBatch();
                }, null, TimeSpan.FromSeconds(timeTrigger), Timeout.InfiniteTimeSpan);
                _transformBlock = new TransformBlock<T, T>(model =>
                {
                    _timer.Change(TimeSpan.FromSeconds(timeTrigger), Timeout.InfiniteTimeSpan);
                    return model;
                }, new ExecutionDataflowBlockOptions() { BoundedCapacity = boundedCapacity });
                _transformBlock.LinkTo(_batchBlock, new DataflowLinkOptions() { PropagateCompletion = true });
            }
        }

        /// <summary>
        /// post 数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool PostValue(T model)
        {
            if (!_timeTrigger)
            {
                return _batchBlock.Post(model);
            }
            return _transformBlock.Post(model);
        }

        /// <summary>
        /// 主动触发数据处理，例如：当数据剩余未达到batchsize 主动触发处理数据
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public void TriggerBatch()
        {
            _batchBlock.TriggerBatch();
        }

        /// <summary>
        /// 返回当前执行总数
        /// </summary>
        /// <returns></returns>
        public int GetBatchSum()
        {
            return _batchBlock.Receive().Count();
        }

        /// <summary>
        /// 主动关闭
        /// </summary>
        /// <returns></returns>
        public void Close()
        {
            if (!_timeTrigger)
            {
                _batchBlock.Complete();
            }
            _transformBlock.Complete();
        }
    }
}
