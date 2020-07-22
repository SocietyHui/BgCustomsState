using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MODEL
{
   public  class UserInfo
    {
        public int id { get; set; }
        public string dfNo { get; set; }

        public string electronicDeclaration { get; set; }

        public string electronicDeclarationTime { get; set; }

        public string computerReview { get; set; }

        public string computerReviewTime { get; set; }

        public string sceneReceipt { get; set; }

        public string sceneReceiptTime { get; set; }

        public string documentaryRelease { get; set; }

        public string documentaryReleaseTime { get; set; }

        public string releaseOfGoods { get; set; }

        public string releaseOfGoodsTime { get; set; }
       
        public string ctime { get; set; }

        public string utime { get; set; }
    }
}
