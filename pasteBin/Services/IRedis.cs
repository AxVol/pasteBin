﻿using pasteBin.Areas.Home.Models;

namespace pasteBin.Services
{
    public interface IRedis
    {
        PasteModel? Get(string key);
        void Set(int timeInterval, IEnumerable<PasteModel> pasts);
        List<PasteModel> GetAll();
        void Update(PasteModel paste);
        void Remove(IEnumerable<PasteModel> pasts);
    }
}
