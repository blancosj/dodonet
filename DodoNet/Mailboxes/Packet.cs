using System;
using System.IO;
using System.Collections;

using System.Net;

namespace DodoNet
{
    /// <summary>
    /// Descripción breve de SocketPacket.
    /// </summary>
    public class Packet : IDisposable
    {
        public short prot = 0;					// protocolo
        public int sizepacket = 0;				// size
        public short typepacket = 0;			// tipo d mensaje

        // datos del mensaje
        private short typemsg = 0;
        public short GetTypeMsg { get { return typemsg; } }

        private long longMsg = 0;
        public long GetLongMsg { get { return longMsg; } }

        // idPacket
        private int idPacket = 0;
        public int GetIdPacket { get { return idPacket; } }
        // idMsg
        private int idMsg = 0;
        public int GetIdMsg { get { return idMsg; } }

        public MemoryStream stPacket = new MemoryStream();
        public BinaryReader stbreader;
        protected BinaryWriter stbwriter;

        private bool disposed = false;

        public Packet()
        {
            stbreader = new BinaryReader(stPacket);
            stbwriter = new BinaryWriter(stPacket);
        }

        public Packet(byte[] bHeader)
        {
            stbreader = new BinaryReader(stPacket);
            stbwriter = new BinaryWriter(stPacket);

            stbwriter.Write(bHeader);

            stPacket.Position = 0;
            prot = stbreader.ReadInt16();
            sizepacket = stbreader.ReadInt32();
            typepacket = stbreader.ReadInt16();
        }

        public Packet(short __prot, int __size, short __typepacket)
        {
            stbreader = new BinaryReader(stPacket);
            stbwriter = new BinaryWriter(stPacket);

            prot = __prot;
            sizepacket = __size;
            typepacket = __typepacket;

            stbwriter.Write(prot);
            stbwriter.Write(sizepacket);
            stbwriter.Write(typepacket);
        }

        /// <summary>
        /// Constructor para ser utilizado por Frame para crear paquetes especificando 
        /// el nodeId del mensaje y el nodeId del paquete
        /// </summary>
        /// <param name="__prot">protocolo</param>
        /// <param name="__size">tamaño de la carga util</param>
        /// <param name="__typepacket">tipo de paquete</param>
        /// <param name="idPacket">nodeId del paquete</param>
        /// <param name="idMsg">nodeId del mensaje</param>
        public Packet(short __prot, int __size, short __typepacket, int __idmsg, int __idpacket)
        {
            stbreader = new BinaryReader(stPacket);
            stbwriter = new BinaryWriter(stPacket);

            prot = __prot;
            // tamaño paquete = tamaño paquete + (int)nodeId mensaje + (int)nodeId paquete
            sizepacket = __size + 8;
            typepacket = __typepacket;
            idMsg = __idmsg;
            idPacket = __idpacket;

            stbwriter.Write(prot);
            stbwriter.Write(sizepacket);
            stbwriter.Write(typepacket);

            stbwriter.Write(idMsg);
            stbwriter.Write(__idpacket);
        }

        public Packet(short __typepacket)
        {
            int size = 0;
            stbreader = new BinaryReader(stPacket);
            stbwriter = new BinaryWriter(stPacket);

            prot = DodoConfig.DefaultProtocol;
            sizepacket = DodoConfig.SizePacket;
            typepacket = __typepacket;

            stbwriter.Write(prot);
            stbwriter.Write(size);
            stbwriter.Write(typepacket);

            // actualizar el tamaño q hay en la cabecera
            UpdateHeader();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prot"></param>
        /// <returns></returns>
        static public int GetNumDataBytes(short prot)
        {
            int ret = 0;
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    ret = DodoConfig.SizePacket - 8;
                    break;
                case Protocols.Prot_HTTP:
                    ret = DodoConfig.SizePacket;
                    break;
            }
            return ret;
        }

        /// <summary>
        /// total de bytes que tiene el paquete header + data
        /// </summary>
        /// <returns></returns>
        public int GetNumBytes()
        {
            return (int)stPacket.Length;
        }

        public byte[] GetBytes()
        {
            stPacket.Position = 0;
            return stbreader.ReadBytes((int)stPacket.Length);
        }

        public int GetDataSize()
        {
            int ret = 0;
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    ret = (int)stPacket.Length - (DodoConfig.SizeHeader + 8);
                    break;
            }
            return ret;
        }

        /// <summary>
        /// Segun la dimension del paquete actualizamos la dimension 
        /// descrita en el header
        /// </summary>
        public void UpdateHeader()
        {
            long oldPos = stPacket.Position;
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    {
                        sizepacket = (int)stPacket.Length - DodoConfig.SizeHeader;
                        stPacket.Position = 0;
                        stbwriter.Write(prot);
                        stbwriter.Write(sizepacket);
                        stbwriter.Write(typepacket);
                        break;
                    }
            }
            // dejamos el puntero donde estaba
            stPacket.Position = oldPos;
        }

        public byte[] GetDataBytes()
        {
            byte[] ret = new byte[0];
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    {
                        stPacket.Position = DodoConfig.SizeHeader + 8;
                        ret = stbreader.ReadBytes((int)(stPacket.Length - stPacket.Position));
                        break;
                    }
            }
            return ret;
        }

        public string GetDataString()
        {
            string ret = "";
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    stPacket.Position = DodoConfig.SizeHeader + 8;
                    break;
            }
            if (stPacket.Length != stPacket.Position)
                ret = stbreader.ReadString();
            return ret;
        }

        /// <summary>
        /// escribe en el buffer de datos de paquete una matriz de bytes
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="count"></param>
        public void Write(byte[] buffer, int offset, int count)
        {
            try
            {
                stPacket.Write(buffer, offset, count);
            }
            catch
            {
                throw new Exception("Error to read headers");
            }
        }

        /// <summary>
        /// escribe en el buffer de datos de paquete la representacion en bytes de un short
        /// </summary>
        /// <param name="value"></param>
        public void Write(short value)
        {
            stbwriter.Write(value);
        }

        /// <summary>
        /// escribe en el buffer de datos de paquete la representacion en bytes de un int
        /// </summary>
        /// <param name="value"></param>
        public void Write(int value)
        {
            stbwriter.Write(value);
        }

        /// <summary>
        /// escribe en el buffer de datos de paquete la representacion en bytes de un long
        /// </summary>
        /// <param name="value"></param>
        public void Write(long value)
        {
            stbwriter.Write(value);
        }

        /// <summary>
        /// escribe en el buffer de datos de paquete la representacion en string de un long
        /// </summary>
        /// <param name="value"></param>
        public void Write(string value)
        {
            stbwriter.Write(value);
        }

        public int GetPending()
        {
            int ret = 0;
            ret = sizepacket - ((int)stPacket.Length - DodoConfig.SizeHeader);
            return ret;
        }

        /// <summary>
        /// completar recepción cargando datos cabecera
        /// </summary>
        public void ReadHead()
        {
            switch (prot)
            {
                case Protocols.Prot_FV_Compress:
                case Protocols.Prot_FV:
                    {
                        switch (typepacket)
                        {
                            case TypePacket.PreviousMsgDataFileStream:
                                // peticion de envio de mensaje
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                typemsg = stbreader.ReadInt16();
                                longMsg = stbreader.ReadInt64();
                                break;
                            case TypePacket.PartialDataFileStream:
                                // peticion de envio de mensaje
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                typemsg = stbreader.ReadInt16();
                                longMsg = stbreader.ReadInt64();
                                break;
                            case TypePacket.PreviousMsgData:
                                // peticion de envio de mensaje
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                typemsg = stbreader.ReadInt16();
                                longMsg = stbreader.ReadInt64();
                                break;
                            case TypePacket.PreviousMsgFileStream:
                                // peticion de envio de mensaje
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                typemsg = stbreader.ReadInt16();
                                longMsg = stbreader.ReadInt64();
                                // __ap.Headers.Add("HASHCODE", __ap.stbreader.ReadBytes(20));
                                break;
                            case TypePacket.PartialData:
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                idPacket = stbreader.ReadInt32();
                                break;
                            case TypePacket.PartialFileStream:
                                stPacket.Position = DodoConfig.SizeHeader;
                                idMsg = stbreader.ReadInt32();
                                idPacket = stbreader.ReadInt32();
                                break;
                        }
                        break;
                    }
            }
        }

        #region Miembros de IDisposable

        public void Dispose()
        {
            try
            {
                if (!disposed)
                {
                    stbreader.Close();
                    stbreader = null;
                    stbwriter.Close();
                    stbwriter = null;
                    stPacket.Close();
                    stPacket = null;

                    disposed = true;
                }
            }
            catch { }
        }

        #endregion
    }
}
