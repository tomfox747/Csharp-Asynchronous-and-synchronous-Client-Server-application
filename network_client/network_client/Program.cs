using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;

namespace network_client
{
    class Program
    {
        static void Main(string[] args)
        {
            //specify the ip address of the server and the port number
            string serverIP = "192.168.0.44";
            int portNumber = 7474;

            //Allow the user to repeatedly send messages
            while(true)
            {
                //ask user for message input
                Console.WriteLine("input your message");
                string Message = Console.ReadLine();

                //Do not allow data formatting characters
                if (Message.Contains('|'))
                {
                    Console.WriteLine("Illegal use of the '|' character");
                }
                else
                {
                    //attempt connection to the tcp client
                    try
                    {
                        //create new tcp client object with the ip and port number of the server
                        TcpClient client = new TcpClient(serverIP, portNumber);

                        //break from program
                        if (Message == "exit") { break; }

                        //add message end character for data transfer
                        Message += "|";

                        //get the size of the message in bytes / create byte array for the message
                        int byteCount = Encoding.ASCII.GetByteCount(Message);
                        byte[] sendData = new byte[byteCount];

                        //convert the message into a byte array
                        sendData = Encoding.ASCII.GetBytes(Message);

                        //create a data stream and write the message to that stream / add timeout for the stream reader
                        NetworkStream stream = client.GetStream();
                        stream.ReadTimeout = 500;
                        stream.Write(sendData, 0, sendData.Length);

                        //create a byte array to store returned inforamtion from the server
                        byte[] returnedInfo = new byte[1000];

                        //checks for response for server using readtimeout
                        try
                        {
                            //read data from the data stream
                            stream.Read(returnedInfo, 0, returnedInfo.Length);

                            //send the data to the function to remove empty bytes
                            byte[] formattedInfo = removeEmptyBytes(returnedInfo);

                            //convert data into a string
                            string returnedMessage = Encoding.ASCII.GetString(formattedInfo, 0, formattedInfo.Length);
                            Console.WriteLine(returnedMessage);
                        }catch(Exception e)
                        {
                            Console.WriteLine("no response from the server - message may have not sent");
                        }

                        //close the stream and the client
                        stream.Close();
                        client.Close();
                    }
                    catch (Exception e)
                    { 
                        Console.WriteLine(e.ToString());
                    }
                }
            }
        }

        public static byte[] removeEmptyBytes(byte[] input)
        {
            string inputString = Encoding.ASCII.GetString(input);
            string outputString = "";
            foreach(char c in inputString)
            {
                if(c == '|')
                {
                    break;
                }
                else
                {
                    outputString += c;
                }
            }
            
            byte[] outputArray = new byte[outputString.Length];
            outputArray = Encoding.ASCII.GetBytes(outputString);
            return outputArray;
        }
    }
}
