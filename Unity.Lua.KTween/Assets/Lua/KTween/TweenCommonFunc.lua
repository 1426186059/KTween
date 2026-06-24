TweenCommonFunc = {}

function TweenCommonFunc:easeLinear(from, to, timePercent)
	return from * (1 - timePercent) + to * timePercent
end

function TweenCommonFunc:easeInOutQuad(from, to, timePercent)
	timePercent = timePercent * 2
	to = to - from;
	if timePercent < 1 then
		return to / 2 * timePercent * timePercent + from;
	end

	timePercent = timePercent - 1;
	return -to / 2 * (timePercent * (timePercent - 2) - 1) + from
end

function TweenCommonFunc:easeInOutSine(from, to, timePercent)
	to = to - from
	return -to / 2 * (math.cos(math.pi * timePercent) - 1) + from;
end

function TweenCommonFunc:easeInOutQuart(from, to, timePercent)
	timePercent = timePercent * 2
	to = to - from;
	if timePercent < 1 then
		return to / 2 * timePercent * timePercent * timePercent * timePercent + from;
	end
	
	timePercent = timePercent - 2;
	return -to / 2 * (timePercent * timePercent * timePercent * timePercent - 2) + from
end




function TweenCommonFunc:easeOutQuad(from, to, timePercent)
	local diff = to - from
	local val = -timePercent * (timePercent - 2)
	return diff * val + from
end

function TweenCommonFunc:easeInQuad(from, to, timePercent)
	local diff = to - from
	local val = timePercent * timePercent
	return diff * val + from
end

function TweenCommonFunc:easeOutSine(from, to, timePercent)
	local diff = to - from
	local val = math.sin(timePercent * math.pi / 2)
	return from + diff * val
end

function TweenCommonFunc:easeInSine(from, to, timePercent)
	local diff = to - from
	local val = -math.cos(timePercent * math.pi / 2)
	return from + diff + diff * val
end

function TweenCommonFunc:tweenOnCurve(from, to, timePercent, animationCurve)
	return from + (to - from) * animationCurve:Evaluate(timePercent)
end

--local LTBezierPath = CS.LTBezierPath(to);
function TweenCommonFunc:tweenOnBezierPath(val, mLTBezierPath)
	return mLTBezierPath:point(val);
end
