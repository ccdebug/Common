using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace MetricsInfluxDb
{
    public class IpHelper
    {
        /// <summary>
        /// 验证IP地址是否是本地IP
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsLocalIp(string ip)
        {
            var isLocal = false;
            var list = GetLocalIpList();
            if (list != null && list.Count > 0)
            {
                isLocal = list.Contains(ip);
            }
            return isLocal;
        }

        /// <summary>
        /// 把IP地址转换为长整数
        /// </summary>
        /// <param name="ip">IP地址</param>
        /// <returns></returns>
        public static long IpToLong(string ip)
        {
            var separator = new char[] { '.' };
            var items = ip.Split(separator);
            return long.Parse(items[0]) << 24
                    | long.Parse(items[1]) << 16
                    | long.Parse(items[2]) << 8
                    | long.Parse(items[3]);
        }

        /// <summary>
        /// 长整形数值，转IP地址格式
        /// </summary>
        /// <param name="ipLong">长整数IP数值</param>
        /// <returns></returns>
        public static string LongToIp(long ipLong)
        {
            var sb = new StringBuilder();
            sb.Append((ipLong >> 24) & 0xFF).Append(".");
            sb.Append((ipLong >> 16) & 0xFF).Append(".");
            sb.Append((ipLong >> 8) & 0xFF).Append(".");
            sb.Append(ipLong & 0xFF);
            return sb.ToString();
        }

        /// <summary>
        /// 获取本机IP地址(多个默认取第一个)
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            var list = GetLocalIpList();
            if (list != null && list.Count > 0) return list[0];
            return string.Empty;
        }

        /// <summary>
        /// 获取本机IP地址列表
        /// </summary>
        /// <returns></returns>
        public static List<string> GetLocalIpList()
        {
            var ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            return (from ip in ipHostEntry.AddressList where IsIp(ip.ToString()) select ip.ToString()).ToList();
        }

        public static string GetAddress(string host, AddressType addressFormat)
        {
            var ipAddress = string.Empty;
            var addrFamily = AddressFamily.InterNetwork;
            switch (addressFormat)
            {
                case AddressType.IPv4:
                    addrFamily = AddressFamily.InterNetwork;
                    break;
                case AddressType.IPv6:
                    addrFamily = AddressFamily.InterNetworkV6;
                    break;
            }
            var ipe = Dns.GetHostEntry(host);
            if (host != ipe.HostName)
            {
                ipe = Dns.GetHostEntry(ipe.HostName);
            }
            foreach (var ipa in ipe.AddressList)
            {
                if (ipa.AddressFamily == addrFamily)
                {
                    return ipa.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIp(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }
    }

    public enum AddressType
    {
        IPv4,
        IPv6
    }
}
