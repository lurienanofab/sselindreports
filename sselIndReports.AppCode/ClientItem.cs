﻿using LNF.Data;
using System.Collections.Generic;

namespace sselIndReports.AppCode
{
    public class ClientItem
    {
        public int ClientID { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string DisplayName { get { return Clients.GetDisplayName(LName, FName); } }
    }

    public class ClientItemEqualityComparer : IEqualityComparer<ClientItem>
    {
        public bool Equals(ClientItem x, ClientItem y)
        {
            return x.ClientID == y.ClientID;
        }

        public int GetHashCode(ClientItem obj)
        {
            return obj.ClientID.GetHashCode();
        }
    }
}
