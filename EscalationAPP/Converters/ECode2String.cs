using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Escalation.World;
using EscalationAPP.Converters;

namespace EscalationAPP.Converters
{
    public class ECode2String : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Ecode ecode = (Ecode)value;
            switch (ecode)
            {
                case Ecode.FRA:
                    return "France";
                case Ecode.ALL:
                    return "Allemagne";
                case Ecode.ESP:
                    return "Espagne";
                case Ecode.ITA:
                    return "Italie";
                case Ecode.ROY:
                    return "Royaume-Uni";
                case Ecode.ETU:
                    return "Etats-Unis";
                case Ecode.RUS:
                    return "Russie";




                default:
                    return "-- NO NAME --";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
