using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace VirtMuseWeb.Utility
{
    public interface ISerializable
    {
        /// <summary>
        /// serialize obj to byte array
        /// </summary>
        /// <returns>byte array of obj</returns>
        byte[] Serialize();
        /// <summary>
        /// serialize object into binary writer
        /// </summary>
        /// <param name="w">the binary writer writen into</param>
        void Serialize(BinaryWriter w);
    }
}