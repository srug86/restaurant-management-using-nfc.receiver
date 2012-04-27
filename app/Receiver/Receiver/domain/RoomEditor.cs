using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Receiver.communication;

namespace Receiver.domain
{
    class RoomEditor
    {
        private AdapterWebServices adapter = AdapterWebServices.Instance;

        private RoomDef room;

        internal RoomDef Room
        {
            get { return room; }
            set { room = value; }
        }

        public void updateData()
        {
        }

        public void saveRoom()
        {
            adapter.sendRoom(xmlRoomBuilder());
        }

        public void loadRoom(string name)
        {
        }

        private string xmlRoomBuilder()
        {
            string xml = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n";
            xml += "<Room name=\"" + room.Name + "\">\n";
            xml += "\t<Dimension>\n";
            xml += "\t\t<Width>" + room.Width + "</Width>\n";
            xml += "\t\t<Height>" + room.Height + "</Height>\n";
            xml += "\t\t<NTables>" + room.getNTables() + "</NTables>\n";
            xml += "\t\t<TCapacity>" + room.getTCapacity() + "</TCapacity>\n";
            xml += "\t</Dimension>\n";
            xml += "\t<Receiver>\n";
            xml += writeBoxes(room.Receiver, false);
            xml += "\t</Receiver>\n";
            xml += "\t<Bar>\n";
            xml += writeBoxes(room.Bar, false);
            xml += "\t</Bar>\n";
            xml += "\t<Tables>\n";
            if (room.getNTables() > 0)
                foreach (Table table in room.Tables)
                {
                    xml += "\t\t<Table id=\"" + table.Id + "\">\n";
                    xml += "\t\t\t<Capacity>" + table.Capacity + "</Capacity>\n";
                    writeBoxes(table.Place, true);
                    xml += "\t\t</Table>\n";
                }
            xml += "\t</Tables>\n";
            xml += "</Room>";
            return xml;
        }

        private string writeBoxes(List<int[]> list, bool extraTab)
        {
            string tab = "";
            if (extraTab) tab = "\t";
            string xml = tab + "\t\t<Boxes>\n";
            foreach (int[] coordinates in list)
            {
                xml += tab + "\t\t\t<Box>\n";
                xml += tab + "\t\t\t\t<X>" + coordinates[1] + "</X>\n";
                xml += tab + "\t\t\t\t<Y>" + coordinates[0] + "</Y>\n";
                xml += tab + "\t\t\t</Box>\n";
            }
            xml += tab + "\t\t</Boxes>\n";
            return xml;
        }

        private void xmlDistributionOfRoom(string xml)
        {
        }

        private void refreshTables()
        {
        }
    }

    public interface SubjectRE
    {
        void registerInterest(ObserverRE obs);
    }

    public interface ObserverRE
    {
        void notifyChangesInABox(int row, int column, int state);
    }
}
