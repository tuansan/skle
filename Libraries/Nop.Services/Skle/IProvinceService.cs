using Nop.Core.Domain.Skle;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nop.Services.Skle
{
    public partial interface IProvinceService
    {
        IEnumerable<Province> GetAllProvince(string KeySearch = null);

        Province GetProvinceById(int id);

        bool Insert(Province item);

        bool Update(Province item);

        bool DeleteProvince(int id);
    }
}