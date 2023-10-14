# JWT密钥问题

1. 相同的密钥，配置代码也相同，在Web应用1中生成JWT时，没有报错，而在Web应用2中生成JWT时，报**密钥长度不够**

   ````c#
   IDX10720: Unable to create KeyedHashAlgorithm for algorithm 'http://www.w3.org/2001/04/xmldsig-more#hmac-sha256', the key size must be greater than: '256' bits, key has '136' bits. (Parameter 'keyBytes')
   ````

   解决：初始时，Web2中安装NuGet包`System.IdentityModel.Tokens.Jwt`，版本`6.15.1`，Web1中未安装此NuGet包，默认引用版本`6.10.0`，这两个版本的NuGet包都不会验证密钥长度，或者要求密钥长度没那么高，由于Web2中升级NugGet包，版本`6.32.1`此时要求验证密钥长度256bits



2. 
3. 