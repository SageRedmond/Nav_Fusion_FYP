from contextlib import asynccontextmanager
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
from pydantic.json import pydantic_encoder
import json
import os
from threading import Lock

anchors_lock = Lock()

class Vector3(BaseModel):
  x: float
  y: float
  z: float

class Anchor(BaseModel):
  id: str
  position: Vector3

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

# Start server: uvicorn main:app --reload
app = FastAPI(lifespan=lifespan)

# if item is of type str: curl -X POST -H "Content-Type: application/json" 'http://127.0.0.1:8000/items?item=orange'
# if item is a json object: curl -X POST -H "Content-Type: application/json" -d '{"text":"apple"}' 'http://127.0.0.1:8000/items'
@app.post("/anchors")
def create_acnhor(anchor: Anchor):
  anchors.append(anchor)
  saveToJson()
  return anchor

# curl -X GET 'http://127.0.0.1:8000/items?limit=3'
@app.get("/anchors", response_model=list[Anchor])
def list_items():
  return anchors