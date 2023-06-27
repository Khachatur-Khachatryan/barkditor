import grpc
from Protos import files_pb2_grpc, projectFiles_pb2_grpc

__channel = grpc.insecure_channel("localhost:5000")
files_client = files_pb2_grpc.FilesStub(__channel)
project_files_client = projectFiles_pb2_grpc.ProjectFilesStub(__channel)
