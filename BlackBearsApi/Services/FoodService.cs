using System.Collections.Generic;
using WebApplication1.Model;

namespace WebApplication1.Services
{
    public class FoodService
    {
        private static readonly object _lockObject = new object();
        
        public List<Food> Food { get; }

        private static volatile FoodService _instance;
        public static FoodService Instance
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
                    _instance = new FoodService();
                }
                return _instance;
            }
        }

        private FoodService()
        {
            Food = new List<Food>();
        }
    }
}