using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SynthTest.Presentation.Views.Tools
{
    // This class acts as a "Bridge" to pass DataContext to detached objects like ContextM
    public class BindingProxy : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new BindingProxy();
        }

        public object Data
        {
            get { 
                return (object)GetValue(DataProperty); 
            }
            set { 
                SetValue(DataProperty, value); 
            }
        }

        public static readonly DependencyProperty DataProperty = 
            DependencyProperty.Register("Data", typeof(object), typeof(BindingProxy), new PropertyMetadata(null));
    }
}
