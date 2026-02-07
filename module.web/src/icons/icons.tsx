import { h } from '@stencil/core';

export class Icon{
    public static add() {
        return <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960">
            <path d="M440-440H200v-80h240v-240h80v240h240v80H520v240h-80v-240Z"/>
        </svg>;
    };

    public static back() {
        return <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 -960 960 960">
            <path d="m313-440 224 224-57 56-320-320 320-320 57 56-224 224h487v80H313Z"/>
        </svg>
    }
};