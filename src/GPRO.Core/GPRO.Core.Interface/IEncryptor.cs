using System;
namespace GPRO.Core.Interface
{
	public interface IEncryptor
	{
		string Encrypt(byte[] data);
		byte[] Decrypt(string data);
		object Deserialize(byte[] bytes);
		byte[] Serialize(object obj);
	}
}
