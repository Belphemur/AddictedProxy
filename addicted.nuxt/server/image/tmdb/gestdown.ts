import { joinURL, encodeQueryItem } from 'ufo'
import {createOperationsGenerator} from "#image";
import {ProviderGetImage, } from "@nuxt/image";


const operationsGenerator = createOperationsGenerator({
    keyMap: {
        width: 'width',
        height: 'height',
        quality: 'quality',
        format: 'format',
        background: 'bgcolor',
    },
    joinWith: '&',
    formatter: (key, val) => encodeQueryItem(key, val)
})

const defaultModifiers = {}

export const getImage: ProviderGetImage = (src, {
    modifiers = {},
} = {}) => {
    const baseURL = useRuntimeConfig().public.api.clientUrl;
    const mergeModifiers = { ...defaultModifiers, ...modifiers }
    const operations = operationsGenerator(mergeModifiers as any)

    const url = operations ? joinURL(baseURL, `${src}?${operations}`) : joinURL(baseURL, src)

    return {
        url
    }
}