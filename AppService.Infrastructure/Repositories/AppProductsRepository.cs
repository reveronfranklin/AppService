﻿using AppService.Core.Entities;
using AppService.Core.Interfaces;
using AppService.Core.QueryFilters;
using AppService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using AppService.Core.DTOs.Shared;
using AppProducts = AppService.Core.Entities.AppProducts;


namespace AppService.Infrastructure.Repositories
{
    public class AppProductsRepository : IAppProductsRepository
    {

        private readonly RRDContext _context;
       
     

        public AppProductsRepository(RRDContext context)
        {
            
            _context = context;
        }

      
    
        
        
        public async Task<List<AppProducts>> GetAll()
        {

            try
            {
                var result = await _context.AppProducts.ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
         
            


        }

        public async Task<List<AppProducts>> GetAllBySubCategory(int subCategoryId)
        {

            try
            {
                var result = await _context.AppProducts.Where(x=>x.AppSubCategoryId==subCategoryId).ToListAsync();
                return result;
            }
            catch (Exception e)
            {
                return null;
            }
         
            


        }
        
         public async Task<ResultDto<List<AppProducts>>> GetAllFilterPaginate(AppProdutsQueryFilter filter)
        {


            filter.PageNumber = filter.PageNumber +1;
            if (filter.PageSize == 0) filter.PageSize = 4000;
            var totalRegistros = 0;
            var totalPage = 0;
            
            ResultDto<List<AppProducts>> result = new ResultDto<List<AppProducts>>(null);
            List<AppProducts> resultList = new List<AppProducts>();
            var resultAll =await  GetAllBySubCategory(filter.SubCategoria);

            try
            {


               
                if (filter.SearchText != null && filter.SearchText.Length > 0)
                {
                    resultList = resultAll.Where(x => x.Description1.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Description2.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Code.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();



                }

                else
                {
                    resultList =resultAll.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                  


                }

                
                totalRegistros = resultAll.Count;

                totalPage = (totalRegistros + filter.PageSize - 1) / filter.PageSize;

                result.Data = resultList;
                result.IsValid = true;
                result.Message = "";
                result.IsValid = true;
                result.CantidadRegistros = totalRegistros;
                result.TotalPage = totalPage;
                if (filter.PageNumber > 0) filter.PageNumber = filter.PageNumber - 1;
                result.Page = filter.PageNumber;
                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }


        }

        
        
        public async Task<List<AppProducts>> GetAllFilter(AppProdutsQueryFilter filter)
        {



            List<AppProducts> result = new List<AppProducts>();
            var resultAll =await  GetAll();

            try
            {


                if (filter.Code != null && filter.Code.Length > 0)
                {
                    result = resultAll.Where(x => x.Code.Trim().ToLower() == filter.Code.Trim().ToLower()).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                }
                else if (filter.Description1 != null && filter.Description1.Length > 0)
                {
                    result = resultAll.Where(x => x.Description1.Trim().ToLower().Contains(filter.Description1.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                }
                else if (filter.Description2 != null && filter.Description2.Length > 0)
                {
                    result =resultAll.Where(x => x.Description2.Trim().ToLower().Contains(filter.Description2.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                }
                else if (filter.SearchText != null && filter.SearchText.Length > 0)
                {
                    if (filter.SubCategoria > 0)
                    {
                        result = resultAll.Where(x => (x.AppSubCategoryId == filter.SubCategoria) && (x.Description1.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Description2.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Code.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                    }
                    else
                    {
                        result = resultAll.Where(x => x.Description1.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Description2.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Code.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
                    }


                }

                else
                {

                    if (filter.SubCategoria > 0)
                    {
                        result = resultAll.Where(x => x.AppSubCategoryId == filter.SubCategoria).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();

                    }
                    else
                    {
                        result =resultAll.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList();
                    }


                }


                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }


        }

             public async Task<List<AppProducts>> GetAllFilterCopia(AppProdutsQueryFilter filter)
        {



            List<AppProducts> result = new List<AppProducts>();
            result =await  GetAll();

            try
            {


                if (filter.Code != null && filter.Code.Length > 0)
                {
                    result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.Code.Trim().ToLower() == filter.Code.Trim().ToLower()).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));

                }
                else if (filter.Description1 != null && filter.Description1.Length > 0)
                {
                    result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.Description1.Trim().ToLower().Contains(filter.Description1.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));

                }
                else if (filter.Description2 != null && filter.Description2.Length > 0)
                {
                    result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.Description2.Trim().ToLower().Contains(filter.Description2.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));

                }
                else if (filter.SearchText != null && filter.SearchText.Length > 0)
                {
                    if (filter.SubCategoria > 0)
                    {
                        result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => (x.AppSubCategoryId == filter.SubCategoria) && (x.Description1.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Description2.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Code.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()))).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));

                    }
                    else
                    {
                        result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.Description1.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Description2.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower()) || x.Code.Trim().ToLower().Contains(filter.SearchText.Trim().ToLower())).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));
                    }


                }

                else
                {

                    if (filter.SubCategoria > 0)
                    {
                        result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.AppSubCategoryId == filter.SubCategoria).Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));

                    }
                    else
                    {
                        result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize));
                    }


                }


                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }


        }
        
        public async Task<List<AppProducts>> GetAllByVarible(List<AppVariableSearchCompareQueryFilter> filter)
        {



            List<AppProducts> result = new List<AppProducts>();


            try
            {

                string searchText = "";

                foreach (var item in filter.OrderBy(x => x.AppVariableId))
                {
                    searchText = searchText + item.AppVariableId.ToString() + item.SearchText.Trim();
                }

                result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.VariablesSearchText.Contains(searchText)));





                return result;

            }
            catch (Exception e)
            {
                var a = e.InnerException.Message;
                return result;
            }


        }


        public async Task<AppProducts> GetById(int id)
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.AppProducts.Where(x => x.Id == id));
        }
        public async Task<AppProducts> GetByCode(string code)
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.AppProducts.Where(x => x.Code == code));
        }

        public async Task<AppProducts> GetFirstByExternalCode(string code)
        {
            return await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(_context.AppProducts.Where(x => x.ExternalCode.Trim() == code.Trim()));
        }

        public async Task Add(AppProducts entity)
        {
            await _context.AppProducts.AddAsync(entity);


        }



        public void Update(AppProducts entity)
        {
            try
            {
                _context.AppProducts.Update(entity);
            }
            catch (Exception ex)
            {
                var msg = ex.InnerException.Message;
                throw;
            }


        }

        public async Task Delete(int id)
        {
            //AppProducts entity = await GetById(id);
            //_context.AppProducts.Remove(entity);


            var idP = new SqlParameter("@Id", id);

            try
            {
                var result = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.FromSqlRaw<AppProducts>("exec AppDeleteProduct @Id", idP));

                var aprobacion = result.FirstOrDefault();
                //return aprobacion;
            }
            catch (Exception ex)
            {
                var mess = ex.InnerException.Message;

                throw;
            }



        }


        public List<AppProductVariableSearchText> GetProductByVariablesSearchText()
        {
            List<AppProductVariableSearchText> result = new List<AppProductVariableSearchText>();





            return result;
        }

        public async Task<List<AppProducts>> GetAllByVarible(
      List<AppVariableSearchCompareQueryFilter> filter,
      int subCategoryId)
        {
            List<AppProducts> result = new List<AppProducts>();
            try
            {
                //List<AppProducts> listAsync = await this._context.AppProducts.Include<AppProducts, ICollection<AppRecipes>>((Expression<Func<AppProducts, ICollection<AppRecipes>>>)(i => i.AppRecipes)).Where<AppProducts>((Expression<Func<AppProducts, bool>>)(x => x.AppSubCategoryId == (int?)subCategoryId)).ToListAsync<AppProducts>();

                var listProducts = await EntityFrameworkQueryableExtensions.ToListAsync(_context.AppProducts.Where(x => x.AppSubCategoryId == subCategoryId));
                if (listProducts != null)
                {
                    foreach (AppProducts appProducts in listProducts)
                    {
                        //List<AppRecipes> list = appProducts.AppRecipes.Where<AppRecipes>((Func<AppRecipes, bool>)(r =>
                        //{
                        //    bool? includeInSearch = r.IncludeInSearch;
                        //    bool flag = true;
                        //    return includeInSearch.GetValueOrDefault() == flag & includeInSearch.HasValue;
                        //})).ToList<AppRecipes>();

                        var list = _context.AppRecipes.Where(r => r.AppproductsId == appProducts.Id && r.IncludeInSearch == true).ToList();
                        if (this.VariablesInRecipes(filter, list))
                            result.Add(appProducts);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                string message = ex.InnerException.Message;
                return result;
            }
        }


        public bool VariablesInRecipes(List<AppVariableSearchCompareQueryFilter> listVariables, List<AppRecipes> recipes)
        {
            bool flag = false;
            foreach (AppVariableSearchCompareQueryFilter listVariable in listVariables)
            {
                AppVariableSearchCompareQueryFilter item = listVariable;
                if (recipes.Where<AppRecipes>((Func<AppRecipes, bool>)(r => r.VariablesSearchText.Trim().Contains(item.AppVariableId.ToString() + item.SearchText.Trim()))).FirstOrDefault<AppRecipes>() != null)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                    return flag;
                }
            }
            return flag;
        }
    }
}
