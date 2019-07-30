using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ByteBank.Service.Cartao
{
    public class CartaoTesteService : ICartaoService
    {
        public string ObterCartaoDeCreditoDeDestaque()
            =>  "Cartão de Crédito Foda";

        public string ObterCartaoDeDebitoDestaque()
            => "Cartão de Débito Foda"; 
    }
}
