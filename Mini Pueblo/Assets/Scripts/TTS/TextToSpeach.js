import { pipeline } from '@xenova/transformers';
import wavefile from 'wavefile';
import fs from 'fs';

async function generateAudio(phrase, characterType, name) {
    var model;
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
        model = 'Error';
    }

    const synthesizer = await pipeline('text-to-speech', model, {
        //quantized: false, // Remove this line to use the quantized version (default)
    });
    
    // Generate speech
    const speaker_embeddings = 'https://huggingface.co/datasets/Xenova/transformers.js-docs/resolve/main/speaker_embeddings.bin';
    const result = await synthesizer(phrase, 
        {
        speaker_embeddings
        }
    );
    
    const wav = new wavefile.WaveFile();
    wav.fromScratch(1, result.sampling_rate, '32f', result.audio);
    fs.writeFileSync(name + '.mp3', wav.toBuffer());

    return model
}

//Colombia (Hombre): ylacombe/mms-spa-finetuned-colombian-monospeaker
//Argentina (Mujer): ylacombe/mms-spa-finetuned-argentinian-monospeaker
//Chile (Hombre): ylacombe/mms-spa-finetuned-chilean-monospeaker
//Default (Hombre): Xenova/mms-tts-spa

console.log(await generateAudio('Hola, esto es una frase de prueba. ¿Suena bien?', 'H', 'audioPrueba'));