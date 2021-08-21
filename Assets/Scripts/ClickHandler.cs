using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    private Field field;

    // Start is called before the first frame update
    void Start()
    {
        field = FindObjectOfType<Field>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D raycastHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            Cell _selectedCell;
            if (raycastHit.transform != null && raycastHit.transform.gameObject.TryGetComponent<Cell>(out _selectedCell))
            {
                field.Rotate(_selectedCell);
            }
        }
    }
}
