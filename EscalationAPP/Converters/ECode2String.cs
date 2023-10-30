using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using Escalation.World;

namespace EscalationAPP.Converters
{
    internal class ECode2String : IValueConverter
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
