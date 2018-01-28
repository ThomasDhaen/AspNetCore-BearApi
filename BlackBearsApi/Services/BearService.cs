using System.Collections.Generic;
using BlackBearApi.Model;

namespace BlackBearApi.Services
{
    public class BearService
    {
        private static readonly object _lockObject = new object();
        
        public List<Bear> Bears { get; }

        private static volatile BearService _instance;
        public static BearService Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                lock (_lockObject)
                {
                    if (_instance != null)
                    {
                        return _instance;
                    }
                    _instance = new BearService();
                }
                return _instance;
            }
        }

        private BearService()
        {
            Bears = new List<Bear>();
        }
    }
}