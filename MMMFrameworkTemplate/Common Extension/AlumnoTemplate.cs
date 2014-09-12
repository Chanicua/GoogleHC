using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

using MMMCommon;

namespace MMMFrameworkTemplate.Common_Extension
{
    public class AlumnoTemplate: Alumno
    {
        public AlumnoTemplate(int ID, string nombre, string apellido, string simbolo)
            : base(ID, nombre, apellido, simbolo)
        { 
        
        }

        public AlumnoTemplate(Alumno al)
            : base(al.ID,al.Nombre,al.Apellido,al.Simbolo)
        { 

        }
    }
}
