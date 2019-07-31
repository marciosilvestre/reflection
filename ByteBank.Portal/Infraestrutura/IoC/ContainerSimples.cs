using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Portal.Infraestrutura.IoC
{
    public class ContainerSimples : IContainer
    {
        private readonly Dictionary<Type, Type> _mapaDeTipos = new Dictionary<Type, Type>();

        // Registrar(typeof(IcambioService), typeof(CambioTesteService))
        // Recuperar(typeof(IcambioSerivce) -> Retorna instancia do tipo CambioTesteService

        public void Registrar(Type tipoOrigem, Type tipoDestino)
        {
            if (_mapaDeTipos.ContainsKey(tipoOrigem))
                throw new InvalidOperationException("Tipo já mapeado");

            VerificarHierarquiaOuLancarExcecao(tipoOrigem, tipoDestino);

            _mapaDeTipos.Add(tipoOrigem, tipoDestino);
        }


        public object Recuperar(Type tipoOrigem)
        {
            var tipoDeOrigemFoiMapeado = _mapaDeTipos.ContainsKey(tipoOrigem);
            if (tipoDeOrigemFoiMapeado)
            {
                var tipoDestino = _mapaDeTipos[tipoOrigem];
                return Recuperar(tipoDestino);
            }

            var construtores = tipoOrigem.GetConstructors();
            var construtorSemParametros =
                construtores.FirstOrDefault(c => c.GetParameters().Any() == false);

            if(construtorSemParametros != null)
            {
                var instanciaDeConstrutorSemParametro = construtorSemParametros.Invoke(new object[0]);
                return instanciaDeConstrutorSemParametro;
            }

            var construtroQueVamosUsar = construtores[0];
            var parametrosDoConstrutor = construtroQueVamosUsar.GetParameters();
            var valoresDosParametros = new object[parametrosDoConstrutor.Count()];

            for (int i = 0; i < parametrosDoConstrutor.Count(); i++)
            {
                var parametro = parametrosDoConstrutor[i];
                var tipoParametro = parametro.ParameterType;

                valoresDosParametros[i] = Recuperar(tipoParametro);
            }

            var instancia = construtroQueVamosUsar.Invoke(valoresDosParametros);
            return instancia;
        }

        private void VerificarHierarquiaOuLancarExcecao(Type tipoOrigem, Type tipoDestino)
        {
            //verificar se destino herda ou implementa a origem

            if(tipoOrigem.IsInterface)
            {
                var tipoDestinoImplementaInterface =
                    tipoDestino
                    .GetInterfaces()
                    .Any(tipoInterface => tipoInterface == tipoOrigem);

                if (!tipoDestinoImplementaInterface)
                    throw new InvalidOperationException("O tipo destino não implementa a interface");
                        
            }
            else
            {
                var tipoDestinoHerdaDoTipoOrigem = tipoDestino.IsSubclassOf(tipoOrigem);

                if (!tipoDestinoHerdaDoTipoOrigem)
                    throw new InvalidOperationException("O tipo destino não herda do tipo de origem");
            }
        }
    }
}
