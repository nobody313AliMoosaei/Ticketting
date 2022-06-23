using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.DTO
{
    public class AddTicketDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string DataFile { get; set; }
        public byte[] FileByte { get; set; }
    }
}
