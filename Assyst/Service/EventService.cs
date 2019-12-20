using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Assyst.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace Assyst.Service
{
    public class EventService
    {
        private MobileContext _db;
        private IMemoryCache _cache;
        
        public EventService(MobileContext context, IMemoryCache memoryCache)
        {
            _db = context;
            _cache = memoryCache;
        }

        public void Initialize()
        {
            if (!_db.Events.Any())
            {
                _db.Events.AddRange(
                );
                _db.SaveChanges();
            }
        }

        public async Task<IEnumerable<EventItem>> GetProducts()
        {
            return await _db.Events.ToListAsync();
        }

        public void AddEvent(EventItem item)
        {
            _db.Events.Add(item);
            int n = _db.SaveChanges();
            if (n > 0)
            {
                _cache.Set(item.id, item, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
                });
            }
        }

        public async Task<EventItem> GetEvent(int id)
        {
            EventItem item;
            if (!_cache.TryGetValue(id, out item))
            {
                item = await _db.Events.FirstOrDefaultAsync(p => p.id == id);
                if (item != null)
                {
                    _cache.Set(item.id, item,
                    new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(5)));
                }
            }
            return item;
        }
    }
}
