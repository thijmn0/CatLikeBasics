using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUGraph : MonoBehaviour
{
    [SerializeField]
    ComputeShader computeShader;

    static readonly int
        positionsId=Shader.PropertyToID("_Positions"),
        resolutionId=Shader.PropertyToID("_Resolution"),
        stepId=Shader.PropertyToID("_Step"),
        timeId=Shader.PropertyToID("_Time");

    [SerializeField]
    Material material;

    [SerializeField]
    Mesh mesh;


    [SerializeField, Range(10,1000)]
    int resolution=10;

    [SerializeField]
    FunctionLibrary.FunctionName function;

    public enum TransitionMode{Cycle,Random}

    [SerializeField]
    TransitionMode transitionMode;

    [SerializeField,Min(0f)]
    float functionDuration=1f,transitionDuration=1f;

    float duration;

    bool transitioning;

    FunctionLibrary.FunctionName transitionFunction;

    ComputeBuffer positionsBuffer;

    void OnEnable(){
        positionsBuffer=new ComputeBuffer(resolution*resolution,3*4);
    }

    void OnDisable(){
        positionsBuffer.Release();
        positionsBuffer=null;
    }

    void Update(){
        duration+=Time.deltaTime;
        if(transitioning){
            if(duration>=transitionDuration){
                duration-=transitionDuration;
                transitioning=false;
            }
        }else if(duration>=functionDuration){
            duration-=functionDuration;
            transitioning=true;
            transitionFunction=function;
            PickNextFuction();
        }
        UpdateFunctionOnGPU();
    }

    void UpdateFunctionOnGPU(){
        float step=2f/resolution;
        computeShader.SetInt(resolutionId,resolution);
        computeShader.SetFloat(stepId,step);
        computeShader.SetFloat(timeId,Time.time);

        computeShader.SetBuffer(0,positionsId,positionsBuffer);

        int groups=Mathf.CeilToInt(resolution/8f);
        computeShader.Dispatch(0,groups,groups,1);

        material.SetBuffer(positionsId,positionsBuffer);
        material.SetFloat(stepId,step);
        var bounds=new Bounds(Vector3.zero,Vector3.one*(2f+2f/resolution));
        Graphics.DrawMeshInstancedProcedural(mesh,0,material,bounds,positionsBuffer.count);
    }

    void PickNextFuction(){
        function=transitionMode==TransitionMode.Cycle?
            FunctionLibrary.GetNextFunctionName(function):
            FunctionLibrary.GetRandomFunctionNameOtherThan(function);
    }
}