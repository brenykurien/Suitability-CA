using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Suitablity_based_CA
{
    class Layer
    {
        public string LayerName, LayerPath;
        public double LayerWeight, LayerGrowth;
        public Layer Load(string name,double weight,string path,double gro)
        {
            this.LayerName = name;
            this.LayerWeight = weight;
            this.LayerPath = path;
            this.LayerGrowth = gro;
            return this;
        }
    }
}
