using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Haberlesme
{
    public class SendPacket
    {
        private MAVLink.MavlinkParse mavparse = new MAVLink.MavlinkParse();

        public void send_param_request_list_t_tcp(TcpClient tcpClient)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_param_request_list_t data = new MAVLink.mavlink_param_request_list_t()
            {
                target_system = 1,
                target_component = 1
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            //mavlinkudp.Send(sendpacket1, sendpacket1.Length, ipEP);
            Debug.WriteLine("Sending PARAM_REQUEST_LIST TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent PARAM_REQUEST_LIST TCP...");
        }


        public void send_mission_clear_all_tcp(TcpClient tcpClient, MAVLink.MAV_MISSION_TYPE mav_mission_type)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_mission_clear_all_t data = new MAVLink.mavlink_mission_clear_all_t()
            {
                target_system = 1,
                target_component = 1,
                mission_type = (byte)mav_mission_type
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_CLEAR_ALL,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending MISSION_CLEAR_ALL TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent MISSION_CLEAR_ALL TCP...");
        }


        public void send_mission_request_list_t_tcp(TcpClient tcpClient, MAVLink.MAV_MISSION_TYPE mav_mission_type)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_mission_request_list_t data = new MAVLink.mavlink_mission_request_list_t()
            {
                target_system = 1,
                target_component = 1,
                mission_type = (byte)mav_mission_type
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending MISSION_REQUEST_LIST TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent MISSION_REQUEST_LIST TCP...");
        }


        public void send_mavlink_mission_request_int_t_tcp(TcpClient tcpClient, int seq, MAVLink.MAV_MISSION_TYPE mav_mission_type)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_mission_request_int_t data = new MAVLink.mavlink_mission_request_int_t()
            {
                seq = (byte)seq,
                target_system = 1,
                target_component = 1,
                mission_type = (byte)mav_mission_type
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_INT,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending MISSION_REQUEST_INT TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent MISSION_REQUEST_INT TCP...");
        }


        public void sen_mavlink_mission_count_t_tcp(TcpClient tcpClient, int number_of_mission_items, MAVLink.MAV_MISSION_TYPE mav_mission_type)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_mission_count_t data = new MAVLink.mavlink_mission_count_t()
            {
                count = (byte)number_of_mission_items,
                target_system = 1,
                target_component = 1,
                mission_type = (byte)mav_mission_type
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_COUNT,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending MISSION_COUNT TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent MISSION_COUNT TCP...");
        }


        public void send_mavlink_mission_item_int_t_tcp(TcpClient tcpClient, float param1, float param2, float param3, float param4, int x, int y, float z, ushort seq, MAVLink.MAV_CMD mav_cmd, byte target_system, byte target_component, MAVLink.MAV_FRAME mav_frame, MAVLink.MAV_MISSION_TYPE mission_type)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_mission_item_int_t data = new MAVLink.mavlink_mission_item_int_t()
            {
                param1 = param1,
                param2 = param2,
                param3 = param3,
                param4 = param4,
                x = x,
                y = y,
                z = z,
                seq = seq,
                command = (byte)mav_cmd,
                target_system = 1,
                target_component = 1,
                frame = (byte)mav_frame,
                current = 0,
                autocontinue = 1,
                mission_type = (byte)mission_type
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_ITEM_INT,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending MISSION_ITEM_INT TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent MISSION_ITEM_INT TCP...");
        }


        public void send_mavlink_command_long_t_tcp(TcpClient tcpClient, float param1, float param2, float param3, float param4, float param5, float param6, float param7, MAVLink.MAV_CMD mav_cmd)
        {
            int seqno = 0;
            NetworkStream networkStream = tcpClient.GetStream();

            MAVLink.mavlink_command_long_t data = new MAVLink.mavlink_command_long_t()
            {
                param1 = param1, // https://discuss.cubepilot.org/t/mavlink-commands-to-set-flight-modes-that-are-not-listed/4526/6
                param2 = param2,  //  ardupilotmega.xml <enum name="COPTER_MODE"> başlığı
                param3 = param3,
                param4 = param4,
                param5 = param5,
                param6 = param6,
                param7 = param7,
                command = (ushort)mav_cmd,
                target_system = 1,
                target_component = 1,
                confirmation = 0
            };

            byte[] sendPacket = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.COMMAND_LONG,
                                                                 data,
                                                                 false,
                                                                 1,
                                                                 1,
                                                                 seqno++);

            Debug.WriteLine("Sending COMMAND_LONG TCP...");
            networkStream.Write(sendPacket, 0, sendPacket.Length);
            Debug.WriteLine("Sent COMMAND_LONG TCP...");
        }

    }
}











////MISSION_REQUEST_LIST
//MAVLink.mavlink_param_request_list_t data = new MAVLink.mavlink_param_request_list_t()
//{
//    target_system = 1,
//    target_component = 1
//};
//byte[] sendpacket1 = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.MISSION_REQUEST_LIST,
//data, false, 1, 1, seqno++);

////mavlinkudp.Send(sendpacket1, sendpacket1.Length, ipEP);
//Console.WriteLine("Sending...");
//stm.Write(sendpacket1, 0, sendpacket1.Length);


//MISSION_CLEAR_ALL



//stm = tcpClient2.GetStream();

//data = new MAVLink.mavlink_param_request_list_t()
//{
//    target_system = 1,
//    target_component = 1
//};
//sendpacket1 = mavparse.GenerateMAVLinkPacket20(MAVLink.MAVLINK_MSG_ID.PARAM_REQUEST_LIST,
//data, false, 1, 1, seqno++);

////mavlinkudp.Send(sendpacket1, sendpacket1.Length, ipEP);
//Console.WriteLine("Sending...");
//stm.Write(sendpacket1, 0, sendpacket1.Length);