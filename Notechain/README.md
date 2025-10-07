# Block chain

## Chain header structure
|  Type  |  Length |    Description   |
|--------|--------:|------------------|
|  Int32 | 4 bytes | Number of blocks |
| UInt32 | 4 bytes | Title length     |
| Byte[] |       - | Title            |

Header length is *256 bytes*. If real length is less than *256 bytes* then the remainder will be filled with zeros.

## Block structure
|   Type   |  Length  |     Description     |
|----------|---------:|---------------------|
| Guid     | 16 bytes | Unique ID           |
| UInt32   |  4 bytes | Height              |
| Byte[]   | 32 bytes | Block hash          |
| Byte[]   | 32 bytes | Previous block hash |
| DateTime |  8 bytes | Timestamp           |
| Int64    |  8 bytes | Nonce value         |
| Int32    |  4 bytes | Difficulty          |
| UInt32   |  4 bytes | Comment length      |
| Byte[]   |     -    | Comment             |
| UInt64   |  8 bytes | Data length         |
| Byte[]   |     -    | Data                |

## Block validation
The block hash has the following structure:
```
sha256
(
  [Guid]
  [Height]
  [Previous hash]
  [Timestamp]
  [Nonce]
  [Difficulty]
  [Comment]
  [Data]
)
```