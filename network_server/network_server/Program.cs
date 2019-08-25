using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace network_server
{
    class Program
    {
        static void Main(string[] args)
        {
            //Specify the ip address and the port of the server - If running remotely, listen on 0.0.0.0. Port number doesn't matter
            string ipAddress = "127.0.0.1";
            int portNumber = 7474;

            //set the ip address / create the server and client objects
            IPAddress ip = IPAddress.Parse(ipAddress);
            TcpListener server = new TcpListener(ip, portNumber);
            TcpClient client = default(TcpClient);

            //attempts to connect to server with a try catch
            try
            {
                //start the server
                server.Start();
                Console.WriteLine("server is running on port : {0}", portNumber);

                //listening loop starts
                while(true)
                {
                    //create a byte array to hold information from the client - create string to hold ASCII converted message
                    byte[] recievedBuffer = new byte[1000];
                    string recievedMessage;

                    //save the client to the server / create a data stream
                    client = server.AcceptTcpClient();
                    NetworkStream stream = client.GetStream();
                    
                    //read data to stream / pass parameters... byte array, position to begin reading, byte array size
                    stream.Read(recievedBuffer, 0, recievedBuffer.Length);
                    
                    //Remove all null bytes in the buffer and save to new array
                    byte[] formattedArray = removeEmptyBytesFromArray(recievedBuffer);
                    //Convert the formatted array to a string
                    recievedMessage = byteArrayToString(formattedArray);
                    //Remove any data transfer formatting from the string
                    recievedMessage = deFormatString(recievedMessage);
                    
                    //Print the information to the console window
                    Console.WriteLine("-- " + recievedMessage + " --");

                    //Return information to the client machine and console / create byte array response to send
                    byte[] response = formatResponse();
                    respondToRequest(stream, response);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        //convert any byte array to a string
        public static string byteArrayToString(byte[] input)
        {
            string output = Encoding.ASCII.GetString(input, 0, input.Length);
            return output;
        }
        //Convert any string to a byte array
        public static byte[] stringToByteArray(string input)
        {
            byte[] outputArray = new byte[Encoding.ASCII.GetByteCount(input)];
            outputArray = Encoding.ASCII.GetBytes(input);
            return outputArray;
        }

        //Add any string formatting required for the data transfer
        public static string formatString(string input)
        {
            input += '|';
            return input;
        }
        //Remove any formatting used for the data transfer
        public static string deFormatString(string input)
        {
            string output = "";

            foreach(char c in input)
            {
                if(c == '|')
                {
                    break;
                }
                else
                {
                    output += c;
                }
            }

            return output;
        }

        //create a response to the client request
        public static byte[] formatResponse()
        {
            byte[] confirmationByteArray = new byte[1000];
            string confirmationMessage = "your message has been recieved";
            confirmationMessage += '|';
            confirmationByteArray = stringToByteArray(confirmationMessage);
            confirmationByteArray = removeEmptyBytesFromArray(confirmationByteArray);

            return confirmationByteArray;
        }

        //Write the response to the data stream
        public static void respondToRequest(NetworkStream stream, byte[] data)
        {
            stream.Write(data, 0, data.Length);
        }
        
        //Remove any empty bytes from a byte array
        public static byte[] removeEmptyBytesFromArray(byte[] input)
        {
            string fullString = byteArrayToString(input);
            string outputString = "";
            foreach(char c in fullString)
            {
                if (c == '|')
                {
                    outputString += '|';
                    break;
                }
                else
                {
                    outputString += c;
                }
            }

            byte[] messageArray = new byte[outputString.Length];
            messageArray = Encoding.ASCII.GetBytes(outputString);

            return messageArray;
        }
    }
}
