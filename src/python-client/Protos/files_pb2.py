# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Protos/files.proto
"""Generated protocol buffer code."""
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import symbol_database as _symbol_database
from google.protobuf.internal import builder as _builder
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from google.protobuf import empty_pb2 as google_dot_protobuf_dot_empty__pb2
from Protos.google.api import annotations_pb2 as Protos_dot_google_dot_api_dot_annotations__pb2


DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x12Protos/files.proto\x12\x05\x46iles\x1a\x1bgoogle/protobuf/empty.proto\x1a#Protos/google/api/annotations.proto\"2\n\rCreateRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\x12\x13\n\x0bisDirectory\x18\x02 \x01(\x08\"D\n\x0bMoveRequest\x12\x0f\n\x07oldPath\x18\x01 \x01(\t\x12\x0f\n\x07newPath\x18\x02 \x01(\t\x12\x13\n\x0bisDirectory\x18\x03 \x01(\x08\"=\n\x18OpenInFileManagerRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\x12\x13\n\x0bisDirectory\x18\x02 \x01(\x08\"2\n\rRemoveRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\x12\x13\n\x0bisDirectory\x18\x02 \x01(\x08\"\x1f\n\x0f\x43opyPathRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\"2\n\rExistsRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\x12\x13\n\x0bisDirectory\x18\x02 \x01(\x08\" \n\x0e\x45xistsResponse\x12\x0e\n\x06\x65xists\x18\x01 \x01(\x08\"0\n\x0b\x43opyRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\x12\x13\n\x0bisDirectory\x18\x02 \x01(\x08\"\x1c\n\x0cPasteRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\"%\n\x15GetFileContentRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\"W\n\x16GetFileContentResponse\x12\x0f\n\x07\x63ontent\x18\x01 \x01(\t\x12,\n\x0b\x63ontentType\x18\x02 \x01(\x0e\x32\x17.Files.FileContentTypes*A\n\x10\x46ileContentTypes\x12\r\n\tPlainText\x10\x00\x12\n\n\x06\x43sharp\x10\x01\x12\x08\n\x04Json\x10\x02\x12\x08\n\x04Html\x10\x03\x32\xce\x05\n\x05\x46iles\x12G\n\x06\x43reate\x12\x14.Files.CreateRequest\x1a\x16.google.protobuf.Empty\"\x0f\x82\xd3\xe4\x93\x02\t\"\x07/Create\x12\x41\n\x04Move\x12\x12.Files.MoveRequest\x1a\x16.google.protobuf.Empty\"\r\x82\xd3\xe4\x93\x02\x07\x1a\x05/Move\x12G\n\x06Remove\x12\x14.Files.RemoveRequest\x1a\x16.google.protobuf.Empty\"\x0f\x82\xd3\xe4\x93\x02\t*\x07/Remove\x12h\n\x11OpenInFileManager\x12\x1f.Files.OpenInFileManagerRequest\x1a\x16.google.protobuf.Empty\"\x1a\x82\xd3\xe4\x93\x02\x14\x12\x12/OpenInFileManager\x12M\n\x08\x43opyPath\x12\x16.Files.CopyPathRequest\x1a\x16.google.protobuf.Empty\"\x11\x82\xd3\xe4\x93\x02\x0b\"\t/CopyPath\x12\x46\n\x06\x45xists\x12\x14.Files.ExistsRequest\x1a\x15.Files.ExistsResponse\"\x0f\x82\xd3\xe4\x93\x02\t\x12\x07/Exists\x12\x41\n\x04\x43opy\x12\x12.Files.CopyRequest\x1a\x16.google.protobuf.Empty\"\r\x82\xd3\xe4\x93\x02\x07\"\x05/Copy\x12\x44\n\x05Paste\x12\x13.Files.PasteRequest\x1a\x16.google.protobuf.Empty\"\x0e\x82\xd3\xe4\x93\x02\x08\"\x06/Paste\x12\x66\n\x0eGetFileContent\x12\x1c.Files.GetFileContentRequest\x1a\x1d.Files.GetFileContentResponse\"\x17\x82\xd3\xe4\x93\x02\x11\x12\x0f/GetFileContentB\x15\xaa\x02\x12\x42\x61rkditor.Protobufb\x06proto3')

_globals = globals()
_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, _globals)
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'Protos.files_pb2', _globals)
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\022Barkditor.Protobuf'
  _FILES.methods_by_name['Create']._options = None
  _FILES.methods_by_name['Create']._serialized_options = b'\202\323\344\223\002\t\"\007/Create'
  _FILES.methods_by_name['Move']._options = None
  _FILES.methods_by_name['Move']._serialized_options = b'\202\323\344\223\002\007\032\005/Move'
  _FILES.methods_by_name['Remove']._options = None
  _FILES.methods_by_name['Remove']._serialized_options = b'\202\323\344\223\002\t*\007/Remove'
  _FILES.methods_by_name['OpenInFileManager']._options = None
  _FILES.methods_by_name['OpenInFileManager']._serialized_options = b'\202\323\344\223\002\024\022\022/OpenInFileManager'
  _FILES.methods_by_name['CopyPath']._options = None
  _FILES.methods_by_name['CopyPath']._serialized_options = b'\202\323\344\223\002\013\"\t/CopyPath'
  _FILES.methods_by_name['Exists']._options = None
  _FILES.methods_by_name['Exists']._serialized_options = b'\202\323\344\223\002\t\022\007/Exists'
  _FILES.methods_by_name['Copy']._options = None
  _FILES.methods_by_name['Copy']._serialized_options = b'\202\323\344\223\002\007\"\005/Copy'
  _FILES.methods_by_name['Paste']._options = None
  _FILES.methods_by_name['Paste']._serialized_options = b'\202\323\344\223\002\010\"\006/Paste'
  _FILES.methods_by_name['GetFileContent']._options = None
  _FILES.methods_by_name['GetFileContent']._serialized_options = b'\202\323\344\223\002\021\022\017/GetFileContent'
  _globals['_FILECONTENTTYPES']._serialized_start=659
  _globals['_FILECONTENTTYPES']._serialized_end=724
  _globals['_CREATEREQUEST']._serialized_start=95
  _globals['_CREATEREQUEST']._serialized_end=145
  _globals['_MOVEREQUEST']._serialized_start=147
  _globals['_MOVEREQUEST']._serialized_end=215
  _globals['_OPENINFILEMANAGERREQUEST']._serialized_start=217
  _globals['_OPENINFILEMANAGERREQUEST']._serialized_end=278
  _globals['_REMOVEREQUEST']._serialized_start=280
  _globals['_REMOVEREQUEST']._serialized_end=330
  _globals['_COPYPATHREQUEST']._serialized_start=332
  _globals['_COPYPATHREQUEST']._serialized_end=363
  _globals['_EXISTSREQUEST']._serialized_start=365
  _globals['_EXISTSREQUEST']._serialized_end=415
  _globals['_EXISTSRESPONSE']._serialized_start=417
  _globals['_EXISTSRESPONSE']._serialized_end=449
  _globals['_COPYREQUEST']._serialized_start=451
  _globals['_COPYREQUEST']._serialized_end=499
  _globals['_PASTEREQUEST']._serialized_start=501
  _globals['_PASTEREQUEST']._serialized_end=529
  _globals['_GETFILECONTENTREQUEST']._serialized_start=531
  _globals['_GETFILECONTENTREQUEST']._serialized_end=568
  _globals['_GETFILECONTENTRESPONSE']._serialized_start=570
  _globals['_GETFILECONTENTRESPONSE']._serialized_end=657
  _globals['_FILES']._serialized_start=727
  _globals['_FILES']._serialized_end=1445
# @@protoc_insertion_point(module_scope)