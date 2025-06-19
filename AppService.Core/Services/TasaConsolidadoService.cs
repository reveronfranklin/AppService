
using System;
using AppService.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;
using AppService.Core.Dtos;
using AppService.Core.DTOs.Cotizaciones;
using AppService.Core.DTOs.Shared;
using AppService.Core.EntitiesMooreve;
using AppService.Core.Utility;

namespace AppService.Core.Services
{
    public class TasaConsolidadoService: ITasaConsolidadoService
    {

        private readonly IUnitOfWork _unitOfWork;
        public TasaConsolidadoService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async  Task<ResultDto<List<TasaConsolidadasResponseDto>>> GetAll()
        {
            
            ResultDto<List<TasaConsolidadasResponseDto>> result = new ResultDto<List<TasaConsolidadasResponseDto>>(null);

            
            List<TasaConsolidadasResponseDto> resultList = new List<TasaConsolidadasResponseDto>();
            var tasas=  await _unitOfWork.TasaConsolidadoRepository.GetAll();


            foreach (var tasa in tasas)
            {
                TasaConsolidadasResponseDto resultItem = new TasaConsolidadasResponseDto();
                resultItem.Id = tasa.Id;
                resultItem.Fecha = tasa.Fecha;
                resultItem.FechaString = tasa.SearchText; 
                FechaDto fechaObj = FechaObj.GetFechaDto(tasa.Fecha);
                resultItem.FechaObj = fechaObj;
                resultItem.TasaBcv = tasa.TasaBcv;
                resultItem.TasaParalelo = tasa.TasaParalelo;
                resultItem.TasaPromedio = tasa.TasaPromedio;
                resultItem.SearchText = tasa.SearchText;
                resultList.Add(resultItem);
            }
            
            result.Data = resultList;
            result.IsValid = true;
            result.Message = "";
            result.CantidadRegistros=tasas.Count;
            result.TotalPage = 1;
            result.Page = 1;
                
            return result;
         
        }
      
        
        public async  Task<ResultDto<bool>> UpdateTasas(TasasConsolidadasUpdateDto tasas)
        {
            
            ResultDto<bool> result = new ResultDto<bool>(false);

            try
            {
                var isValidDate = IsValidDateGeneral.IsValidDate(tasas.Year, tasas.Month, tasas.Day);
                if (isValidDate==false)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Datos de Fecha Invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                if (tasas.TasaBcv<=0)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Tasa BCV Invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                if (tasas.TasaParalelo<=0)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Tasa Paralelo Invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
                
                if (tasas.TasaPromedio<=0)
                {
                   tasas.TasaPromedio = (tasas.TasaBcv+tasas.TasaParalelo)/2;
                }
                
                var tasasResult=  await _unitOfWork.TasaConsolidadoRepository.UpdateTasaConsolidada(tasas);
                if (tasasResult == "")
                {
                    result.Data = true;
                    result.IsValid = true;
                    result.Message = "";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
                else
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = tasasResult;
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
            }
            catch (Exception e)
            {
                result.Data = false;
                result.IsValid = false;
                result.Message = e.Message;
                result.CantidadRegistros = 1;
                result.TotalPage = 1;
                result.Page = 1;
            }
    
           
           
                
            return result;
         
        }


   public async  Task<ResultDto<bool>> DeleteTasas(TasasConsolidadasUpdateDto tasas)
        {
            
            ResultDto<bool> result = new ResultDto<bool>(false);

            try
            {
                var isValidDate = IsValidDateGeneral.IsValidDate(tasas.Year, tasas.Month, tasas.Day);
                if (isValidDate==false)
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = "Datos de Fecha Invalida";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                    return result;
                }
             
            
             
                var tasasResult=  await _unitOfWork.TasaConsolidadoRepository.DeleteTasaConsolidada(tasas);
                if (tasasResult == "")
                {
                    result.Data = true;
                    result.IsValid = true;
                    result.Message = "";
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
                else
                {
                    result.Data = false;
                    result.IsValid = false;
                    result.Message = tasasResult;
                    result.CantidadRegistros = 1;
                    result.TotalPage = 1;
                    result.Page = 1;
                }
            }
            catch (Exception e)
            {
                result.Data = false;
                result.IsValid = false;
                result.Message = e.Message;
                result.CantidadRegistros = 1;
                result.TotalPage = 1;
                result.Page = 1;
            }
    
           
           
                
            return result;
         
        }




    }
}
