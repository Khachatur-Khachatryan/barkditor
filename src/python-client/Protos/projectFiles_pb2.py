# -*- coding: utf-8 -*-
# Generated by the protocol buffer compiler.  DO NOT EDIT!
# source: Protos/projectFiles.proto
"""Generated protocol buffer code."""
from google.protobuf.internal import builder as _builder
from google.protobuf import descriptor as _descriptor
from google.protobuf import descriptor_pool as _descriptor_pool
from google.protobuf import symbol_database as _symbol_database
# @@protoc_insertion_point(imports)

_sym_db = _symbol_database.Default()


from google.protobuf import empty_pb2 as google_dot_protobuf_dot_empty__pb2
from Protos.google.api import annotations_pb2 as Protos_dot_google_dot_api_dot_annotations__pb2


DESCRIPTOR = _descriptor_pool.Default().AddSerializedFile(b'\n\x19Protos/projectFiles.proto\x12\x0cProjectFiles\x1a\x1bgoogle/protobuf/empty.proto\x1a#Protos/google/api/annotations.proto\"%\n\x15SetProjectPathRequest\x12\x0c\n\x04path\x18\x01 \x01(\t\"&\n\x16GetProjectPathResponse\x12\x0c\n\x04path\x18\x01 \x01(\t2\xdf\x01\n\x0cProjectFiles\x12\x66\n\x0eSetProjectPath\x12#.ProjectFiles.SetProjectPathRequest\x1a\x16.google.protobuf.Empty\"\x17\x82\xd3\xe4\x93\x02\x11\x12\x0f/SetProjectPath\x12g\n\x0eGetProjectPath\x12\x16.google.protobuf.Empty\x1a$.ProjectFiles.GetProjectPathResponse\"\x17\x82\xd3\xe4\x93\x02\x11\x12\x0f/GetProjectPathB\x15\xaa\x02\x12\x42\x61rkditor.Protobufb\x06proto3')

_builder.BuildMessageAndEnumDescriptors(DESCRIPTOR, globals())
_builder.BuildTopDescriptorsAndMessages(DESCRIPTOR, 'Protos.projectFiles_pb2', globals())
if _descriptor._USE_C_DESCRIPTORS == False:

  DESCRIPTOR._options = None
  DESCRIPTOR._serialized_options = b'\252\002\022Barkditor.Protobuf'
  _PROJECTFILES.methods_by_name['SetProjectPath']._options = None
  _PROJECTFILES.methods_by_name['SetProjectPath']._serialized_options = b'\202\323\344\223\002\021\022\017/SetProjectPath'
  _PROJECTFILES.methods_by_name['GetProjectPath']._options = None
  _PROJECTFILES.methods_by_name['GetProjectPath']._serialized_options = b'\202\323\344\223\002\021\022\017/GetProjectPath'
  _SETPROJECTPATHREQUEST._serialized_start=109
  _SETPROJECTPATHREQUEST._serialized_end=146
  _GETPROJECTPATHRESPONSE._serialized_start=148
  _GETPROJECTPATHRESPONSE._serialized_end=186
  _PROJECTFILES._serialized_start=189
  _PROJECTFILES._serialized_end=412
# @@protoc_insertion_point(module_scope)
