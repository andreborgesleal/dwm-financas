using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using App_Dominio.Component;
using System;
using System.Collections.Generic;

namespace DWM.Models.Repositories
{
    public class TransferenciaBancariaViewModel : Repository
    {
        public MovtoBancarioViewModel movtoBancarioOrigemViewModel { get; set; }
        public MovtoBancarioViewModel movtoBancarioDestinoViewModel { get; set; }
    }
}