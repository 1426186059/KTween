TweenMgr = {}
function TweenMgr:Init()
	if self.mTweenT == nil then
		self.mTweenT = {}
		TimeMgr:AddListener(self)
	end
end

function TweenMgr:Clear()
	TimeMgr:RemoveListener(self)
	self.mTweenT = nil
end

function TweenMgr:Update()
	local deltaTime = TimeMgr.deltaTime
	for i = #self.mTweenT, 1, -1 do
		local mItem = self.mTweenT[i]

		if mItem.toggle == false or 
			(mItem.bBindObj and LuaHelper.OrGameObjectExist(mItem.bindObj) == false) then
			
			table.remove(self.mTweenT, i)
		elseif mItem.delay > 0 then
			mItem.delay = mItem.delay - deltaTime
		else
			if mItem.nLoopPingTong > 0 then
                if mItem.nLoopPingTong == 2 then
                    mItem.time = mItem.time - deltaTime
                else
                    mItem.time = mItem.time + deltaTime;
				end
				
				mItem.time = LuaHelper.Clamp(mItem.time, 0, mItem.sumTime)
				local fercent = mItem.time / mItem.sumTime
				local updateFunc = mItem.updateFunc
				if updateFunc then
					updateFunc(fercent)
				end	

				if mItem.nLoopPingTong == 2 then
					if fercent <= 0 then
						mItem.nLoopPingTong = 1;
                        if mItem.nLoopCount > 0 then
                            mItem.nLoopCount = mItem.nLoopCount - 1
                            if mItem.nLoopCount <= 0 then
								local finishFunc = mItem.finishFunc
								if finishFunc then
									finishFunc()
								end

								table.remove(self.mTweenT, i)
							end
                        end
					end
				else
					if fercent >= 1.0 then
						mItem.nLoopPingTong = 2
					end
				end
			else
				mItem.time = mItem.time + deltaTime
				mItem.time = LuaHelper.Clamp(mItem.time, 0, mItem.sumTime)

				local fercent = mItem.time / mItem.sumTime
				local updateFunc = mItem.updateFunc
				if updateFunc then
					updateFunc(fercent)
				end

				if fercent >= 1 then
					if mItem.nLoopCount == -1 then
                        mItem.time = 0
                    else
                        mItem.nLoopCount = mItem.nLoopCount - 1;
                        if mItem.nLoopCount > 0 then
							mItem.time = 0
						else
							local finishFunc = mItem.finishFunc
							if finishFunc then
								finishFunc()
							end
							table.remove(self.mTweenT, i)
						end
                    end
				end

			end
		end
	end
end

function TweenMgr:AddTween(time, updateFunc, finishFunc)
	self:Init()
	local mItem = LuaHelper.CreateNewInstance(self.ItemGenerator)
	mItem:Init()
	mItem.toggle = true
	mItem.time = 0
	mItem.sumTime = math.max(time, 0.001)
	mItem.updateFunc = updateFunc
	mItem.finishFunc = finishFunc
	table.insert(self.mTweenT, mItem)
	return mItem
end

function TweenMgr:AddGoTween(go, time, updateFunc, finishFunc)
	self:Init()
	local mItem = LuaHelper.CreateNewInstance(self.ItemGenerator)
	mItem:Init()
	mItem.toggle = true
	mItem.bBindObj = true
	mItem.bindObj = go
	mItem.time = 0
	mItem.sumTime = math.max(time, 0.001)
	mItem.updateFunc = updateFunc
	mItem.finishFunc = finishFunc
	table.insert(self.mTweenT, mItem)
	return mItem
end

function TweenMgr:delayedCall(time, finishFunc)
	return self:AddTween(time, nil, finishFunc)
end

function TweenMgr:delayedCallWithGo(go, time, finishFunc)
	return self:AddGoTween(go, time, nil, finishFunc)
end

-------------------------------------------------------------------------------------
local TweenItem = {}
function TweenItem:Init()
	self.bBindObj = false
	self.bindObj = nil
	self.toggle = false
	self.delay = 0
	self.time = 0
	self.sumTime = 0
	self.updateFunc = nil
	self.finishFunc = nil
	self.nLoopCount = 0
	self.nLoopPingTong = 0
end

function TweenItem:cancel()
	self.toggle = false
    return self
end

function TweenItem:SetDelay(fTime)
	self.delay = fTime;
	return self
end

function TweenItem:SetLoop(nLoopCount)
	if nLoopCount == nil then
		nLoopCount = -1
	end

	self.nLoopCount = nLoopCount;
	return self;
end

function TweenItem:SetLoopPingPong(nLoopCount)
	if nLoopCount == nil then
		nLoopCount = -1
	end

	self.nLoopCount = nLoopCount
	self.nLoopPingTong = 1
	return self;
end

function TweenItem:AppendTween(mTweenItem)
	local fSumTime =  self.sumTime
	if self.nLoopPingTong > 0 then
		fSumTime = fSumTime * 2 * self.nLoopCount
	elseif self.nLoopCount > 0 then
		fSumTime = fSumTime * self.nLoopCount
	end

	local mTweenSumTime = self.delay + fSumTime;
	mTweenItem.delay = mTweenItem.delay + mTweenSumTime;
	return self
end

function TweenItem:AppendSeqTween(mTweenItem)
	self:AppendTween(mTweenItem)
	return mTweenItem
end

function TweenItem:AppendSeqTween1(mTweenItem)
	local nRemoveIndex = LuaHelper.indexOfTable(TweenMgr.mTweenT, mTweenItem)
	Debug.Assert(nRemoveIndex >= 1, "Error: ", nRemoveIndex)
	table.remove(TweenMgr.mTweenT, nRemoveIndex)
	
	local finishFunc = self.finishFunc
	self.finishFunc = function()
		if finishFunc then
			finishFunc()
		end
		table.insert(TweenMgr.mTweenT, mTweenItem)
	end
	return mTweenItem
end

TweenMgr.ItemGenerator = TweenItem




