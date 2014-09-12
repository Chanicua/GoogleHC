using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMMCommon;

namespace MMMFrameworkTemplate.Common_Extension
{
    public class SesionTemplate: Sesion
    {
        public SesionTemplate(int numero, int predecesora, DateTime fechaInicio, bool guardada)
            : base(numero, predecesora, fechaInicio, guardada)
        {
        }
    }
}
