using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationUtility : MonoBehaviour {

	public float _angle;
	public Vector3 _axis;
	public Transform targetTrans;

	private Vector3 _rotateVectorTarget = Vector3.zero;

	private Vector3 rotateY;

	private float t = 0;

	// Use this for initialization
	void Start () {
//		Vector3 aimDirection = (targetTrans.position - transform.position).normalized;
//		Quaternion rot = Quaternion.LookRotation(Vector3.forward, aimDirection);
//		transform.rotation = rot;
	}
	
	// Update is called once per frame
	void Update () {
		t += Time.deltaTime;
		Quaternion from = Quaternion.identity;
		Quaternion to = Quaternion.AngleAxis(180, Vector3.right);
		Quaternion lerpRot = Quaternion.Lerp(from,to, t / 5);
		Quaternion slerpRot = Quaternion.Slerp(from,to, t / 5);

		Vector3 lerpVec = lerpRot * Vector3.up;
		Vector3 slerpVec = slerpRot * Vector3.up;

		//Debug.DrawLine(Vector3.zero, lerpVec * 5, Color.yellow);
		//Debug.DrawLine(Vector3.forward, Vector3.forward + slerpVec * 5, Color.red);

		if(Input.GetKeyDown(KeyCode.Alpha1))
		{
			RotateAngleAxisImmediately(_angle, _axis);
		}

		if(Input.GetKeyDown(KeyCode.Alpha2))
		{
			RotateAngleAxis(_angle, _axis);
		}

		if(Input.GetKey(KeyCode.Alpha3))
		{
			RotateAngleAxisStep(_angle, _axis);
		}

		if(Input.GetKeyDown(KeyCode.Alpha4))
		{
			Vector3 aimDirection = (targetTrans.position - transform.position).normalized;
			PointAtImmediately(transform.up, aimDirection);
		}

		if(Input.GetKeyDown(KeyCode.Alpha5))
		{
			Vector3 aimDirection = (targetTrans.position - transform.position).normalized;
			PointAt(transform.up, aimDirection);
		}

		if(Input.GetKey(KeyCode.Alpha6))
		{
			Vector3 aimDirection = (targetTrans.position - transform.position).normalized;
			PointAtStep(transform.up, aimDirection);
		}

		if(Input.GetKeyDown(KeyCode.Alpha7))
		{
			RotateVectorImmediately(transform.right, -15f, Vector3.one);
			Debug.DrawRay(Vector3.zero, transform.right, Color.yellow, 1f);
		}

		if(Input.GetKeyDown(KeyCode.Alpha8))
		{
			RotateVector(transform.right, -90f, Vector3.one);
		}

		if(Input.GetKey(KeyCode.Alpha9))
		{
			RotateVectorStep(transform.right, 90f, Vector3.up);
			Debug.DrawRay(Vector3.zero, transform.right, Color.yellow, 1f);
		}

		if(Input.GetKeyUp(KeyCode.Alpha9))
		{
			_rotateVectorTarget = Vector3.zero;
		}

		if(Input.GetKey(KeyCode.Alpha0))
		{
			RotateVectorFree(transform.right, Vector3.up);
		}

		Debug.DrawRay(Vector3.zero, transform.right, Color.yellow, 1f);
		Debug.DrawLine(Vector3.zero, Vector3.one * 10, Color.red);
		//Debug.DrawLine(Vector3.zero, Vector3.up * 5, Color.green);
		//Debug.DrawLine(Vector3.zero, rotateY * 5, Color.yellow);
	}

	//在当前朝向的基础上，绕某个轴旋转一定角度（立即指向目标角度）
	void RotateAngleAxisImmediately(float angle, Vector3 axis)
	{
		Quaternion rot = Quaternion.AngleAxis(angle, axis);
		transform.rotation = rot * transform.rotation;
	}

	//在当前朝向的基础上，绕某个轴以一定的角速度旋转（每次执行旋转角度：角速度 * Time.deltaTime）
	void RotateAngleAxisStep(float angularSpeed, Vector3 axis)
	{
		Quaternion rot = Quaternion.AngleAxis(angularSpeed * Time.deltaTime, axis);
		transform.rotation = rot * transform.rotation;
	}

	//在当前朝向的基础上，绕某个轴旋转一定角度（使用协程逐渐逼近目标角度，一次调用即可到达目标角度）
	void RotateAngleAxis(float angle, Vector3 axis)
	{
		StopCoroutine("RotateTowardsCoroutine");
		Quaternion rot = Quaternion.AngleAxis(angle, axis);
		Quaternion targetRot = rot * transform.rotation;
		StartCoroutine("RotateTowardsCoroutine", targetRot);
	}

	IEnumerator RotateTowardsCoroutine(Quaternion targetRotation)
	{
		while(transform.rotation != targetRotation)
		{
			Quaternion stepRot = Quaternion.RotateTowards(transform.rotation, targetRotation, 360 * Time.deltaTime);
			transform.rotation = stepRot;
			yield return null;
		}
	}

	//将自己的某个轴立即指向指定方向
	void PointAtImmediately(Vector3 from, Vector3 to)
	{
		Quaternion targetRot = RotationBetweenVectors(from, to);
		transform.rotation = targetRot;
	}

	//将自己的某个轴指向指定方向（使用协程逼近，一次调用即可到达目标角度）
	//旋转在转动轴和目标方向的所在平面内完成
	void PointAt(Vector3 from, Vector3 to)
	{
		StopCoroutine("RotateTowardsCoroutine");		
		Quaternion targetRot = RotationBetweenVectors(from, to);
		StartCoroutine("RotateTowardsCoroutine", targetRot);
	}

	//将自己的某个轴以一定的角速度逐渐指向指定方向（每次执行旋转角度：角速度 * Time.deltaTime）
	void PointAtStep(Vector3 from, Vector3 to, float angularSpeed = 360f)
	{
		Quaternion targetRot = RotationBetweenVectors(from, to);
		Quaternion stepRot = Quaternion.RotateTowards(transform.rotation, targetRot, angularSpeed * Time.deltaTime);
		transform.rotation = stepRot;
	}

	//计算两个向量之间的旋转角度，辅助函数
	Quaternion RotationBetweenVectors(Vector3 from, Vector3 to)
	{
		float angle = Vector3.Angle(from, to);
		Vector3 cross = Vector3.Cross(from, to);
		Quaternion rot = Quaternion.AngleAxis(angle, cross);
		Quaternion targetRot = rot * transform.rotation;
		return targetRot;
	}

	//立即旋转自身，旋转量可以让自己的某个方向绕某个轴旋转一定的角度
	void RotateVectorImmediately(Vector3 from, float angle, Vector3 axis)
	{
		Vector3 to = RotateVectorAroundAxis(from, angle, axis);
		Quaternion rot = Quaternion.FromToRotation(from, to);
		Quaternion targetRot = rot * transform.rotation;
		transform.rotation = targetRot;
	}

	//逐渐旋转自身，旋转量可以让自己的某个方向绕某个轴旋转一定的角度（使用协程逼近，一次调用即可到达目标角度）
	void RotateVector(Vector3 from, float angle, Vector3 axis)
	{
		StopCoroutine("RotateTowardsCoroutine");
		Vector3 to = RotateVectorAroundAxis(from, angle, axis);
		Quaternion rot = Quaternion.FromToRotation(from, to);
		Quaternion targetRot = rot * transform.rotation;
		StartCoroutine("RotateTowardsCoroutine", targetRot);
	}

	//逐渐旋转自身，旋转量可以让自己的某个方向绕某个轴旋转一定的角度，到达目标角度后锁定（每次执行旋转角度：角速度 * Time.deltaTime）
	void RotateVectorStep(Vector3 from, float angle, Vector3 axis, float angularSpeed = 360f)
	{
		if(_rotateVectorTarget == Vector3.zero)
		{
			_rotateVectorTarget = RotateVectorAroundAxis(from, angle, axis);
		}
		PointAtStep(from, _rotateVectorTarget, angularSpeed);
	}

	//逐渐旋转自身，旋转量可以让自己的某个方向绕某个轴旋转，不锁定（每次执行旋转角度：角速度 * Time.deltaTime）
	void RotateVectorFree(Vector3 from, Vector3 axis, float angularSpeed = 360f)
	{
		Vector3 to = RotateVectorAroundAxis(from, angularSpeed * Time.deltaTime, axis);
		PointAtImmediately(from, to);
	}

	//求出一个向量绕某个轴旋转一定角度后得到的向量，辅助函数
	Vector3 RotateVectorAroundAxis(Vector3 from, float angle, Vector3 axis)
	{
		Quaternion rot = Quaternion.AngleAxis(angle, axis);
		Vector3 to = rot * from;
		return to;
	}
}
