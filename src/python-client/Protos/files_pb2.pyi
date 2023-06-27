from google.protobuf import empty_pb2 as _empty_pb2
from Protos.google.api import annotations_pb2 as _annotations_pb2
from google.protobuf.internal import enum_type_wrapper as _enum_type_wrapper
from google.protobuf import descriptor as _descriptor
from google.protobuf import message as _message
from typing import ClassVar as _ClassVar, Optional as _Optional, Union as _Union

DESCRIPTOR: _descriptor.FileDescriptor

class FileContentTypes(int, metaclass=_enum_type_wrapper.EnumTypeWrapper):
    __slots__ = []
    PlainText: _ClassVar[FileContentTypes]
    Csharp: _ClassVar[FileContentTypes]
    Json: _ClassVar[FileContentTypes]
    Html: _ClassVar[FileContentTypes]
PlainText: FileContentTypes
Csharp: FileContentTypes
Json: FileContentTypes
Html: FileContentTypes

class CreateRequest(_message.Message):
    __slots__ = ["path", "isDirectory"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    path: str
    isDirectory: bool
    def __init__(self, path: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class MoveRequest(_message.Message):
    __slots__ = ["oldPath", "newPath", "isDirectory"]
    OLDPATH_FIELD_NUMBER: _ClassVar[int]
    NEWPATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    oldPath: str
    newPath: str
    isDirectory: bool
    def __init__(self, oldPath: _Optional[str] = ..., newPath: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class OpenInFileManagerRequest(_message.Message):
    __slots__ = ["path", "isDirectory"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    path: str
    isDirectory: bool
    def __init__(self, path: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class RemoveRequest(_message.Message):
    __slots__ = ["path", "isDirectory"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    path: str
    isDirectory: bool
    def __init__(self, path: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class CopyPathRequest(_message.Message):
    __slots__ = ["path"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    path: str
    def __init__(self, path: _Optional[str] = ...) -> None: ...

class ExistsRequest(_message.Message):
    __slots__ = ["path", "isDirectory"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    path: str
    isDirectory: bool
    def __init__(self, path: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class ExistsResponse(_message.Message):
    __slots__ = ["exists"]
    EXISTS_FIELD_NUMBER: _ClassVar[int]
    exists: bool
    def __init__(self, exists: bool = ...) -> None: ...

class CopyRequest(_message.Message):
    __slots__ = ["path", "isDirectory"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    ISDIRECTORY_FIELD_NUMBER: _ClassVar[int]
    path: str
    isDirectory: bool
    def __init__(self, path: _Optional[str] = ..., isDirectory: bool = ...) -> None: ...

class PasteRequest(_message.Message):
    __slots__ = ["path"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    path: str
    def __init__(self, path: _Optional[str] = ...) -> None: ...

class GetFileContentRequest(_message.Message):
    __slots__ = ["path"]
    PATH_FIELD_NUMBER: _ClassVar[int]
    path: str
    def __init__(self, path: _Optional[str] = ...) -> None: ...

class GetFileContentResponse(_message.Message):
    __slots__ = ["content", "contentType"]
    CONTENT_FIELD_NUMBER: _ClassVar[int]
    CONTENTTYPE_FIELD_NUMBER: _ClassVar[int]
    content: str
    contentType: FileContentTypes
    def __init__(self, content: _Optional[str] = ..., contentType: _Optional[_Union[FileContentTypes, str]] = ...) -> None: ...
