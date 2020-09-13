using System.Collections.Concurrent;
using Utils;

namespace Broker
{
    static class DataStorage
    {
        // MARK: Properties
        private static ConcurrentQueue<Data> _payloadQueue;

        // MARK: Initializers
        static DataStorage()
        {
            _payloadQueue = new ConcurrentQueue<Data>();
        }

        // MARK: Methods
        public static void AddNewElement(Data element)
        {
            _payloadQueue.Enqueue(element);
        }

        public static Data GetNextElement()
        {
            Data data = null;

            _payloadQueue.TryDequeue(out data);

            return data;
        }

        public static bool IsEmpty()
        {
            return _payloadQueue.IsEmpty;
        }
    }
}
