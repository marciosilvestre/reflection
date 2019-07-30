using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ByteBank.Portal.Infraestrutura
{
    public abstract class ControllerBase
    {
        protected string View([CallerMemberName]string nomeArquivo = null)
        {
            var type = GetType();
            var diretorioNome = type.Name.Replace("Controller", "");

            var nomeCompletoResource = $"ByteBank.Portal.View.{diretorioNome}.{nomeArquivo}.html";
            var assembly = Assembly.GetExecutingAssembly();

            var streamRecurso = assembly.GetManifestResourceStream(nomeCompletoResource);

            var streamLeitura = new StreamReader(streamRecurso);
            var textoPagina = streamLeitura.ReadToEnd();

            return textoPagina;
        }

        protected string View(object modelo, [CallerMemberName]string nomeArquivo = null)
        {
            // Usamos a outra sobrecarga deste método para recuperarmos o conteúdo
            // bruto de nossa view.
            var viewBruta = View(nomeArquivo);

            // Recuperamos todas as propriedades do modelo.
            var todasAsPropriedadesDoModelo = modelo.GetType().GetProperties();

            // Criamos nossa expressão regular para  obter  os  valores  inseridos
            // entre um par de chaves duplas, como {{prop_1}} ou {{prop_2}}.
            var regex = new Regex("\\{{(.*?)\\}}");
            var viewProcessada =
                // O  método   Regex::Replace(string, MatchEvaluator)  executa   o
                // delegate  MatchEvaluator  para   cada   Match   encontrado   na
                // ViewBruta e substitui seu valor de  acordo  com  o  retorno  de
                // nossa expressão lambda.
                regex.Replace(viewBruta, (match) =>
                {
            // Verificamos com a ajuda do LinqPAD que o nome da propriedade
            // é acessível à partir do valor  do  segundo  grupo  de  nosso 
            // match.
            var nomePropriedade = match.Groups[1].Value;

            // Operamos nossa lista de todas as propriedades do modelo usando
            // o operador linq Single com segurança, pois sabemos  que  todas
            // as classes possuem propriedades com  nomes  únicos,  ou  seja,
            // os nomes de propriedades não se repetem.
            var propriedade = todasAsPropriedadesDoModelo.Single(prop => prop.Name == nomePropriedade);

            // Com nosso PropertyInfo em mãos, basta  executarmos   o  método
            // PropertyInfo::GetValue(object) usando como argumento o  modelo
            // recebido em nossa nova sobrecarga View(object, string).
            var valorBruto = propriedade.GetValue(modelo);

            // O delegate MatchEvaluator  exige  como  tipo  de  retorno  uma
            // string. Como medida de  segurança,  usamos  o  null-coalescing
            // operator   para   não   termos    uma    exceção    do    tipo
            // NullReferenceException.
            return valorBruto?.ToString();
                });

            // Enfim, retornamos nossa view processada!
            return viewProcessada;
        }

    }
}
