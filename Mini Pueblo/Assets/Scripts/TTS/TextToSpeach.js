import { pipeline } from '@xenova/transformers';
import wavefile from 'wavefile';
import fs from 'fs';

async function generateAudio(phrase, characterType, name, path) {
    var model;

    //Selecciono el modelo de audio de forma aleatoria en función del sexo del personaje
    if (characterType == 'H') {
        const randomNumber = Math.floor(Math.random() * 3) + 1;

        if (randomNumber == 1) {
            model = 'ylacombe/mms-spa-finetuned-colombian-monospeaker';
        }
        else if (randomNumber == 2) {
            model = 'ylacombe/mms-spa-finetuned-chilean-monospeaker';
        }
        else {
            model = 'Xenova/mms-tts-spa';
        }
    }
    else if (characterType == 'M') {
        model = 'ylacombe/mms-spa-finetuned-argentinian-monospeaker';
    }
    else {
        console.error("Error: LLamada incorrecta. El sexo del personaje debe ser H(Hombre) o M(Mujer).");
        model = 'Error';
        return model;
    }

    const synthesizer = await pipeline('text-to-speech', model, {
        //quantized: false, // Remove this line to use the quantized version (default)
    });
    
    //Genero el audio utilizando el modelo deseado
    const speaker_embeddings = 'https://huggingface.co/datasets/Xenova/transformers.js-docs/resolve/main/speaker_embeddings.bin';
    const result = await synthesizer(phrase, 
        {
        speaker_embeddings
        }
    );
    
    //Guardo el audio en path
    const wav = new wavefile.WaveFile();
    wav.fromScratch(1, result.sampling_rate, '32f', result.audio);
    fs.writeFileSync(path + name + '.wav', wav.toBuffer());

    return model
}

//Obtengo los argumentos necesarios para ejecutar la función
const args = process.argv.slice(2);

if (args.length != 4) {
    console.error("Error: LLamada incorrecta. Ejemplo de llamada: node TextToSpeach.js <frase> <sexo> <nombre> <directorio>");
    process.exit(1);
}
else {
    generateAudio(args[0], args[1], args[2], args[3])
}

//Colombia (Hombre): ylacombe/mms-spa-finetuned-colombian-monospeaker
//Argentina (Mujer): ylacombe/mms-spa-finetuned-argentinian-monospeaker
//Chile (Hombre): ylacombe/mms-spa-finetuned-chilean-monospeaker
//Default (Hombre): Xenova/mms-tts-spa