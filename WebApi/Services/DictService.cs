using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebApi.Models;

namespace WebApi.Services
{
    public class DictService
    {
        private readonly ConcurrentDictionary<string, DictGroup> _dicts
            = new ConcurrentDictionary<string, DictGroup>(StringComparer.OrdinalIgnoreCase);

        public List<DictGroup> GetAll()
        {
            return _dicts.Values.ToList();
        }

        public DictGroup GetByCode(string code)
        {
            return _dicts.TryGetValue(code, out var group) ? group : null;
        }

        public void AddGroup(string code, string name)
        {
            _dicts[code] = new DictGroup { Code = code, Name = name };
        }

        public bool DeleteGroup(string code)
        {
            return _dicts.TryRemove(code, out _);
        }

        public bool AddItem(string dictCode, DictItem item)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return false;
            item.DictCode = dictCode;
            group.Items.Add(item);
            return true;
        }

        public bool UpdateItem(string dictCode, string itemCode, DictItem item)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return false;
            var index = group.Items.FindIndex(i => i.ItemCode == itemCode);
            if (index < 0) return false;
            item.DictCode = dictCode;
            group.Items[index] = item;
            return true;
        }

        public bool DeleteItem(string dictCode, string itemCode)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return false;
            return group.Items.RemoveAll(i => i.ItemCode == itemCode) > 0;
        }

        public List<DictItem> GetItems(string dictCode)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return new List<DictItem>();
            return group.Items.Where(i => i.Enabled).OrderBy(i => i.SortOrder).ToList();
        }

        public List<DictItem> GetAllItems(string dictCode)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return new List<DictItem>();
            return group.Items.OrderBy(i => i.SortOrder).ToList();
        }

        public string GetItemName(string dictCode, string itemCode)
        {
            if (!_dicts.TryGetValue(dictCode, out var group)) return null;
            return group.Items.FirstOrDefault(i => i.ItemCode == itemCode)?.ItemName;
        }
    }
}
