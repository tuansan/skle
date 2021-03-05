using Nop.Core.Domain.Skle;
using Nop.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nop.Services.Skle
{
    public partial class ProvinceService : IProvinceService
    {
        #region Fields

        private readonly IRepository<Province> _provinceRepository;

        #endregion Fields

        #region Ctor

        public ProvinceService(IRepository<Province> provinceRepository)
        {
            _provinceRepository = provinceRepository;
        }

        #endregion Ctor

        #region Methods

        #region Province

        public IEnumerable<Province> GetAllProvince(string KeySearch = null)
        {
            var query = _provinceRepository.Table;
            if (!string.IsNullOrEmpty(KeySearch))
            {
                query = query.Where(s => s.Name.Contains(KeySearch));
            }
            return query;
        }

        public Province GetProvinceById(int id)
        {
            if (id == 0)
                throw new ArgumentNullException(nameof(Province));
            return _provinceRepository.GetById(id);
        }

        public bool Insert(Province item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Province));
            _provinceRepository.Insert(item);

            return true;
        }

        public bool Update(Province item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(Province));
            _provinceRepository.Update(item);

            return true;
        }

        public bool DeleteProvince(int id)
        {
            var item = _provinceRepository.GetById(id);

            if (item == null)
                throw new ArgumentNullException(nameof(Province));

            _provinceRepository.Delete(item);

            return true;
        }

        #endregion Province

        #endregion Methods
    }
}