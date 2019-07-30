using ByteBank.Portal.Filtros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Portal.Infraestrutura.Binding.Filtros
{
    class FilterResolver
    {
        public FilterResult VerificarFiltros(ActionBindInfo actionBindInfo)
        {
            var methodInfo = actionBindInfo.MethodInfo;

            var atributos = methodInfo.GetCustomAttributes(typeof(FiltroAttribute), false);

            foreach (FiltroAttribute filtro in atributos)
                if (!filtro.PodeContinuar())
                    return new FilterResult(false);

            return new FilterResult(true);


        }
    }
}
