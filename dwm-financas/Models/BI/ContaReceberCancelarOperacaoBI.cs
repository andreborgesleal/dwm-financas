using App_Dominio.Component;
using App_Dominio.Contratos;
using App_Dominio.Entidades;
using App_Dominio.Models;
using App_Dominio.Security;
using DWM.Models.Entidades;
using DWM.Models.Persistence;
using DWM.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace DWM.Models.BI
{
    public class ContaReceberCancelarOperacaoBI : OperacaoCancelarBI<ContaReceberViewModel, 
                                                                     ContaReceberParcelaViewModel,
                                                                     ContaReceberParcelaEventoViewModel,
                                                                     ContaReceberModel,
                                                                     ContaReceberParcelaCrudModel,
                                                                     ContaReceberParcelaEventoModel,
                                                                     ContaReceber,
                                                                     ContaReceberParcela,
                                                                     ContaReceberParcelaEvento>
    {
    }
}