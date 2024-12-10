import torch
import sys
from diffusers import StableDiffusionPipeline, DPMSolverMultistepScheduler
from PIL import Image
import numpy as np

def generateImage(prompt):
    model_id = "stabilityai/stable-diffusion-2-1"

    pipe = StableDiffusionPipeline.from_pretrained(model_id, torch_dtype=torch.float16)
    pipe.scheduler = DPMSolverMultistepScheduler.from_config(pipe.scheduler.config)
    #pipe.scheduler = EulerDiscreteScheduler.from_config(pipe.scheduler.config)
    #pipe.scheduler = PNDMScheduler.from_config(pipe.scheduler.config)
    if torch.cuda.is_available():
        torch.cuda.empty_cache()
        pipe = pipe.to("cuda")
    
    image = pipe(prompt, num_inference_steps=65, guidance_scale=7, height=512, width=512).images[0]
    image.save(f"astronaut_rides_horse_mars.png")

    # Convertir la imagen a escala de grises
    grayscale_image = image.convert("L")  # "L" convierte la imagen a escala de grises
    
    # Guardar la imagen en escala de grises
    grayscale_image.save("image_grayscale.png")

    # Convertir a un array de NumPy para procesarlo
    grayscale_array = np.array(grayscale_image)
    
    # Aplicar un umbral: todo lo que no sea negro (intensidad > 50) se convierte en blanco
    threshold = 30
    binary_array = np.where(grayscale_array > threshold, 255, 0).astype(np.uint8)
    
    # Convertir de vuelta a una imagen de PIL
    binary_image = Image.fromarray(binary_array)
    
    # Guardar la imagen binaria
    binary_image.save("image_black_only.png")

    if torch.cuda.is_available():
        torch.cuda.empty_cache()
    del pipe

    return "Se ha generado la imagen corectamente"


if __name__ == "__main__":
    # Leer el prompt desde la línea de comandos
    if len(sys.argv) > 1:
        prompt = sys.argv[1]
        print(generateImage(prompt))
    else:
        print("No prompt provided.")