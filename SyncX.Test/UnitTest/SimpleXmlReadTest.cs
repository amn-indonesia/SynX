using FluentAssertions;
using SynX.Core;
using SynX.Core.Config;
using SynX.FileAdapter.SimpleXml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SynX.Test.UnitTest
{
    public class SimpleXmlReadTest
    {
        private Dictionary<string,object> GenerateDictionaryStructure()
        {
            var result = new Dictionary<string, object>();
            result.Add("ID_No", "SAP202109270001");
            result.Add("TransactionDate", "2021-09-27 10:28:53");
            result.Add("Plant", "SAP");

            List<Dictionary<string, object>> details = new List<Dictionary<string, object>>();

            var detail = new Dictionary<string, object>();
            detail.Add("PONo", "4500068672");
            detail.Add("VendorCode", "3001729");
            detail.Add("VendorName", "Vendor A");
            detail.Add("OrderNo", "DN4120110015505A");
            detail.Add("DelDate", "2020-11-02");
            detail.Add("DelTime", "03:00:00");
            detail.Add("DelCycle", "1");
            detail.Add("RecStatus", "CLOSE");
            var detailItem = new Dictionary<string, object>();
            detailItem.Add("RecDate", "2020-11-02");
            detailItem.Add("RecTime", "01:31:35");
            detailItem.Add("PartNo", "86160-BZ180-00");
            detailItem.Add("QtyOrder", "30");
            detailItem.Add("QtyReceive", "30");
            detailItem.Add("QtyBalance", "0");
            detailItem.Add("CancelStatus", "0");
            var detailItems1 = new List<Dictionary<string, object>>();
            detailItems1.Add(detailItem);
            detail.Add("Item", detailItems1);
            details.Add(detail);

            var detail2 = new Dictionary<string, object>();
            detail2.Add("PONo", "4500068672");
            detail2.Add("VendorCode", "3001729");
            detail2.Add("VendorName", "Vendor A");
            detail2.Add("OrderNo", "DN4120110015506A");
            detail2.Add("DelDate", "2020-11-02");
            detail2.Add("DelTime", "03:00:00");
            detail2.Add("DelCycle", "1");
            detail2.Add("RecStatus", "CLOSE");
            var detailItem2 = new Dictionary<string, object>();
            detailItem2.Add("RecDate", "2020-11-02");
            detailItem2.Add("RecTime", "01:31:44");
            detailItem2.Add("PartNo", "86160-BZ180-00");
            detailItem2.Add("QtyOrder", "15");
            detailItem2.Add("QtyReceive", "15");
            detailItem2.Add("QtyBalance", "0");
            detailItem2.Add("CancelStatus", "0");
            var detailItems2 = new List<Dictionary<string, object>>();
            detailItems2.Add(detailItem2);
            detail2.Add("Item", detailItems2);
            details.Add(detail2);

            var detail3 = new Dictionary<string, object>();
            detail3.Add("PONo", "4500068672");
            detail3.Add("VendorCode", "3001729");
            detail3.Add("VendorName", "Vendor A");
            detail3.Add("OrderNo", "DN4120110015509A");
            detail3.Add("DelDate", "2020-11-02");
            detail3.Add("DelTime", "02:30:00");
            detail3.Add("DelCycle", "1");
            detail3.Add("RecStatus", "CLOSE");
            var detailItem3 = new Dictionary<string, object>();
            detailItem3.Add("RecDate", "2020-11-02");
            detailItem3.Add("RecTime", "01:11:51");
            detailItem3.Add("PartNo", "86160-BZ180-00");
            detailItem3.Add("QtyOrder", "150");
            detailItem3.Add("QtyReceive", "150");
            detailItem3.Add("QtyBalance", "0");
            detailItem3.Add("CancelStatus", "0");
            var detailItems3 = new List<Dictionary<string, object>>();
            detailItems3.Add(detailItem3);
            detail3.Add("Item", detailItems3);
            details.Add(detail3);

            result.Add("Details", details);

            return result;
        }

        [Fact]
        public void ReadParentChildGrandChild_Return_NestedDictionary()
        {
            var expected = GenerateDictionaryStructure();
            var config = new SyncConfig()
            {
                SyncTypeTag = "GR"
            };

            string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitTest","Files", "parent_child_grandchild.xml");
            IFileAdapter adapter = new SimpleXmlFileAdapter();
            var actual = adapter.ReadSyncFile(fileName, config);
            actual.Should().NotBeNull();
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void WriteParentChildGrandChild_Return_StringXml()
        {
            var fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "UnitTest", "Files", "parent_child_grandchild.xml");
            var expected = System.IO.File.ReadAllText(fileName);
            var config = new SyncConfig()
            {
                SyncTypeTag = "GR"
            };
            var payload = GenerateDictionaryStructure();

            IFileAdapter adapter = new SimpleXmlFileAdapter();
            var actual = adapter.GenerateSyncFile(payload, config);
            actual.Should().Be(expected);
        }
    }
}
