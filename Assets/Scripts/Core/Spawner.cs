﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Shape[] m_allShapes;
    public Transform[] m_queuedXforms = new Transform[3];

    Shape[] m_queuedShapes = new Shape[3];

    float m_queueScale = 0.5f;

    public ParticleSystem m_spawnFx;

    void Awake()
    {
        InitQueue();
    }

    Shape GetRandomShape()
    {
        int i = Random.Range(0, m_allShapes.Length);
        if (!m_allShapes[i])
        {
            Debug.Log("WARNING! Invalid shape!");
            return null;
        }

        return m_allShapes[i];
    }

    public Shape SpawnShape()
    {
        Shape shape = null;

        shape = GetQueuedShape();
        shape.transform.position = transform.position;
        
        StartCoroutine(GrowShape(shape, transform.position, 0.25f));

        if (m_spawnFx)
        {
            m_spawnFx.Play();
        }

        if (!shape)
        {
            Debug.LogWarning("WARNING! Invalid shape in spawner!");
            return null;
        }

        return shape;
    }

    void InitQueue()
    {
        for (int i = 0; i < m_queuedShapes.Length; i++)
        {
            m_queuedShapes[i] = null;
        }

        FillQueue();
    }

    void FillQueue()
    {
        for (int i = 0; i < m_queuedShapes.Length; i++)
        {
            if (m_queuedShapes[i]) continue;

            m_queuedShapes[i] = Instantiate(GetRandomShape(), transform.position, Quaternion.identity) as Shape;
            m_queuedShapes[i].transform.position = m_queuedXforms[i].position + m_queuedShapes[i].m_queueOffset;
            m_queuedShapes[i].transform.localScale = new Vector3(m_queueScale, m_queueScale, m_queueScale);
        }
    }

    Shape GetQueuedShape()
    {
        Shape firstShape = null;

        if (m_queuedShapes[0])
        {
            firstShape = m_queuedShapes[0];
        }

        for (int i = 1; i < m_queuedShapes.Length; i++)
        {
            m_queuedShapes[i - 1] = m_queuedShapes[i];
            m_queuedShapes[i - 1].transform.position = m_queuedXforms[i - 1].position + m_queuedShapes[i - 1].m_queueOffset;
        }

        m_queuedShapes[m_queuedShapes.Length - 1] = null;

        FillQueue();

        return firstShape;
    }

    IEnumerator GrowShape(Shape shape, Vector3 position, float growTime = 0.5f)
    {
        float size = 0f;

        growTime = Mathf.Clamp(growTime, 0.1f, 2f);

        float sizeDelta = 1f / growTime * Time.deltaTime;

        while (size < 1f)
        {
            shape.transform.localScale = new Vector3(size, size, size);
            size += sizeDelta;
            shape.transform.position = position;
            yield return null;
        }

        shape.transform.localScale = Vector3.one;
    }
}
