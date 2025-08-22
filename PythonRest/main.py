# conda activate PythonREST
# ifconfig | grep "inet "
# 

from contextlib import asynccontextmanager
from fastapi import FastAPI, HTTPException, Request
from pydantic import BaseModel
from pydantic.json import pydantic_encoder
import json
import os
from threading import Lock
import logging

anchors_lock = Lock()

logger = logging.getLogger('uvicorn.error')
logger.setLevel(logging.DEBUG)

class Vector3(BaseModel):
  x: float
  y: float
  z: float

class Anchor(BaseModel):
  id: str
  position: Vector3

class anchorsModel(BaseModel):
  anchors: list[Anchor] = []
  

def makeFilePath() -> str:
  folderPath = os.getcwd()
  os.makedirs(folderPath, exist_ok=True)
  file_name = "Anchors.json"
  file_path = os.path.join(folderPath, file_name)
  return file_path

anchorJSONFilePath = makeFilePath()
anchors: list[Anchor] = []

def saveToJson():
  with anchors_lock: # Thread safety
    model_dict = [model.model_dump() for model in anchors]
    with open(anchorJSONFilePath, 'w') as json_file:
      json.dump(model_dict, json_file, indent=1)
  
def loadFromJson() -> list[Anchor]:
  if os.path.exists(anchorJSONFilePath):
    with open(anchorJSONFilePath, 'r') as json_file:
      return [Anchor(**data) for data in json.load(json_file)]
  else:
    return []

@asynccontextmanager
async def lifespan(app: FastAPI):
  global anchors
  print("Startup")
  anchors = loadFromJson()
  yield
  print("Shutdown")
  saveToJson()

# Start server during dev: uvicorn main:app --reload
# Start server localy: uvicorn main:app
# Start server to listen on all IP's: uvicorn main:app --host 0.0.0.0 --port 8080
# Enter http://0.0.0.0:80 or http://localhost:80 as url into browser to connect
# Use ifconfig | grep "inet" on mac to get ip address (Starts with 192)
app = FastAPI(lifespan=lifespan)

# if item is of type str: curl -X POST -H "Content-Type: application/json" 'http://127.0.0.1:8000/items?item=orange'
# if item is a json object: curl -X POST -H "Content-Type: application/json" -d '{"text":"apple"}' 'http://127.0.0.1:8000/items'
@app.put("/anchors")
def create_anchor(anchor: Anchor, request: Request):
  logger.debug("test")
  anchors.append(anchor)
  saveToJson()
  return anchor

@app.get("/anchors", response_model=anchorsModel)
def list_items():
  anchorsList = anchorsModel(anchors=anchors)
  return anchorsList

@app.get("/")
def root():
    return {"Hello" : "World"}