using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeastSquare : MonoBehaviour
{

    //https://www.youtube.com/watch?v=JvS2triCgOY
    protected List<int> x_table;
    protected List<int> y_table;
    private List<int> x_square_table;
    private List<int> xy_table;

    void populateXSquare() {

        for (int i = 0; i < x_table.Count; i++)
        {
            x_square_table.Add(x_table[i] * x_table[i]);
        }
    }

    void populateXY()
    {

        for (int i = 0; i < x_table.Count; i++)
        {
            xy_table.Add(x_table[i] * y_table[i]);
        }
    }

    int somatoria(List<int> coluna) {
        int result = 0;
        for (int i = 0; i < coluna.Count; i++)
        {
            result =+ coluna[i];
        }

        return result;
    }

    int media(List<int> coluna) {
        int media = 0;
        
        media = somatoria(coluna) / coluna.Count;

        return media;
    }

    List<int> desvio;
    List<int> desvioDaReta(List<int> coluna) {
        
        int mediaValue = media(coluna);

        for (int i = 0; i < coluna.Count; i++)
        {
            desvio.Add(coluna[i] - mediaValue);
        }

        return desvio;
    }



    int determinante() {
        int delta = 0;
        int[,] delta_matriz = new int[2,2];
        delta_matriz[0, 0] = x_table.Count;
        delta_matriz[0, 1] = somatoria(x_table);
        delta_matriz[1, 0] = somatoria(x_table);
        delta_matriz[1, 1] = somatoria(x_square_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }

    int determinanteA() {
        int delta = 0;
        int[,] delta_matriz = new int[2, 2];
        delta_matriz[0, 0] = somatoria(y_table);
        delta_matriz[0, 1] = somatoria(xy_table);
        delta_matriz[1, 0] = somatoria(x_table);
        delta_matriz[1, 1] = somatoria(x_square_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }

    int determinanteB() {
        int delta = 0;
        int[,] delta_matriz = new int[2, 2];
        delta_matriz[0, 0] = somatoria(y_table);
        delta_matriz[0, 1] = somatoria(xy_table);
        delta_matriz[1, 0] = somatoria(y_table);
        delta_matriz[1, 1] = somatoria(xy_table);

        delta = (-1 * (delta_matriz[0, 0] * delta_matriz[1, 1]) + (delta_matriz[0, 1] * delta_matriz[1, 0]));

        return delta;
    }

    float lastSquareResult() {
        float y = 0, a, b;

        a = determinanteA() / determinante();
        b = determinanteB() / determinante();

        y = a+b;

        return y;
    }
}
