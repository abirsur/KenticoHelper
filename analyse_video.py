import uuid
import cv2
import numpy as np
import os
import requests
from imagehash import phash
from PIL import Image
from pydantic import BaseModel
from langchain.schema import BaseOutputParser
from pydantic import BaseModel
import requests

def download_video(storage_url, output_path):
    """
    Downloads a video from the given storage account URL.
    """
    response = requests.get(storage_url, stream=True)
    if response.status_code == 200:
        with open(output_path, 'wb') as file:
            for chunk in response.iter_content(chunk_size=1024):
                file.write(chunk)
        print(f"Video downloaded: {output_path}")
    else:
        raise Exception(f"Failed to download video. Status code: {response.status_code}")

def is_similar(hash1, hash2, threshold=5):
    """
    Determines if two perceptual hashes are similar within the given threshold.
    """
    return hash1 - hash2 <= threshold

def extract_representative_frames(video_path, output_dir, interval=30, similarity_threshold=5):
    """
    Extracts representative frames from a video by combining interval sampling and perceptual hashing.
    Saves the frames as JPEG images and returns a list of saved file paths.
    """
    if not os.path.exists(output_dir):
        os.makedirs(output_dir)

    cap = cv2.VideoCapture(video_path)
    frame_count = 0
    saved_frames = []
    prev_hash = None
    extracted_frame_count = 0

    while True:
        ret, frame = cap.read()
        if not ret:
            break

        # Process every nth frame based on interval
        if frame_count % interval == 0:
            # Convert frame to PIL image and compute perceptual hash
            pil_frame = Image.fromarray(cv2.cvtColor(frame, cv2.COLOR_BGR2RGB))
            current_hash = phash(pil_frame)

            # Save frame only if it's not too similar to the previous one
            if prev_hash is None or not is_similar(prev_hash, current_hash, threshold=similarity_threshold):
                extracted_frame_count += 1
                output_file = os.path.join(output_dir, f"frame_{extracted_frame_count:04d}.jpg")
                cv2.imwrite(output_file, frame)
                saved_frames.append(output_file)
                prev_hash = current_hash

        frame_count += 1

    cap.release()
    print(f"Extracted {extracted_frame_count} representative frames.")
    return saved_frames

def image_to_base64(image_path):
    """
    Converts an image file to a Base64 string.
    """
    with open(image_path, "rb") as img_file:
        base64_str = base64.b64encode(img_file.read()).decode('utf-8')
    return base64_str

#  ==================================================== Pydantic parser for the inference API response ====================================================
class GadgetConditionResponse(BaseModel):
    gadget_condition_score: float
    condition_details: str

class GadgetConditionParser(BaseOutputParser):
    def parse(self, text: str) -> GadgetConditionResponse:
        """
        Parses the output text into the Pydantic model.
        """
        return GadgetConditionResponse.parse_raw(text)

#  ==================================================== Function to call the custom inference API  ====================================================
def call_inference_api(base64_image):
    """
    Calls the inference API with the Base64-encoded image and returns the parsed response.
    """
    url = "https://your-inference-api-url.com/analyze"  # Replace with your API endpoint
    headers = {"Content-Type": "application/json"}
    payload = {"image_data": base64_image}

    response = requests.post(url, json=payload, headers=headers)
    if response.status_code == 200:
        # Use the GadgetConditionParser to parse the API response
        parser = GadgetConditionParser()
        parsed_response = parser.parse(response.text)
        return parsed_response
    else:
        raise Exception(f"API call failed with status code {response.status_code}: {response.text}")



# Usage example
if __name__ == "__main__":
    video_filename = uuid.uuid4()
    storage_url = "https://videos.pexels.com/video-files/5000003/5000003-uhd_2560_1440_30fps.mp4"  # Replace with your video URL
    video_output_path = f"{video_filename}.mp4"
    frames_output_dir = uuid.uuid4()
    # Step 1: Download video
    download_video(storage_url, video_output_path)
    # Step 2: Extract unique frames
    frames = extract_representative_frames(video_output_path, frames_output_dir)

    print("Saved frames:", frames)
